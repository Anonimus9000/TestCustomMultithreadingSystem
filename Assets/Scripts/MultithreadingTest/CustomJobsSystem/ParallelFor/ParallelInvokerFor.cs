using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultithreadingTest.CustomJobsSystem.ParallelFor
{
    public class ParallelInvokerFor
    {
        private readonly Dispatcher.Dispatcher _dispatcher;
        private List<Action> _actionsUpdate;

        public ParallelInvokerFor(Dispatcher.Dispatcher dispatcher, int capacity = 10000)
        {
            _dispatcher = dispatcher;
            _actionsUpdate = new List<Action>(capacity);
        }

        public void Invoke<TInput>(IParallelJobFor<TInput>[] jobsFor, ParallelJobTime parallelJobTime)
        {
            if (parallelJobTime == ParallelJobTime.Frame)
            {
                InvokeUpdate(jobsFor);
            }
            else if (parallelJobTime == ParallelJobTime.LateFrame)
            {
                InvokeLateUpdate(jobsFor);
            }
        }

        private void InvokeUpdate<TInput>(IParallelJobFor<TInput>[] jobsFor)
        {
            Parallel.ForEach(jobsFor, job =>
            {
                try
                {
                    var output = job.OtherThreadFunc.Invoke();
                    _dispatcher.InvokeInFrame(() => job.MainThreadAction.Invoke(output));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

        private void InvokeLateUpdate<TInput>(IParallelJobFor<TInput>[] jobsFor)
        {
            
        }
    }
}