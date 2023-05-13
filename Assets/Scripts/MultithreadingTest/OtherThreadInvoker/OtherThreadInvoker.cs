using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MultithreadingTest.ParallelFor;

namespace MultithreadingTest.OtherThreadInvoker
{
    public class OtherThreadInvoker
    {
        private readonly Queue<Action> _tempTimeActions;
        private readonly List<Action> _infinityTimeActions;
        private readonly CancellationToken _mainCancellationToken;
        private readonly Dispatcher _dispatcher;

        public OtherThreadInvoker(CancellationToken mainCancellationToken, Dispatcher dispatcher, int capacity = 10000)
        {
            _mainCancellationToken = mainCancellationToken;
            _dispatcher = dispatcher;
            _tempTimeActions = new Queue<Action>(capacity);
            _infinityTimeActions = new List<Action>(capacity);
            RunInfinityInvoker();
        }

        public void Invoke<TInput>(IParallelJobFor<TInput> job, JobTime jobTime)
        {
            if (jobTime == JobTime.TempTime)
            {
                InvokeTempJob(job);
            }
            else if (jobTime == JobTime.Infinity)
            {
                InvokeInfinityJob(job);
            }
        }
        
        private void InvokeInfinityJob<TInput>(IParallelJobFor<TInput> job)
        {
            _infinityTimeActions.Add(() =>
            {
                var output = job.OtherThreadFunc.Invoke();
                _dispatcher.InvokeInFrame(() => job.MainThreadAction?.Invoke(output));
            });
        }

        private void InvokeTempJob<TInput>(IParallelJobFor<TInput> job)
        {
            _tempTimeActions.Enqueue(() =>
            {
                var output = job.OtherThreadFunc.Invoke();
                _dispatcher.InvokeInFrame(() => job.MainThreadAction?.Invoke(output));
            });
        }

        private void RunInfinityInvoker()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    foreach (var action in _infinityTimeActions)
                    {
                        action?.Invoke();
                    }
                }
            }, _mainCancellationToken);
        }

        private void RunUnknownTimeInvoker()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_tempTimeActions.Count > 0)
                    {
                        while (_tempTimeActions.Count > 0)
                        {
                            _tempTimeActions.Dequeue()?.Invoke();
                        }
                    }
                }
            }, _mainCancellationToken);
        }
    }
}