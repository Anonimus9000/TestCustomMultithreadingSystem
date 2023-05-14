using System.Collections.Generic;
using System.Threading;
using MultithreadingTest.CustomJobsSystem.OtherThreadInvoker.JobThreads;

namespace MultithreadingTest.CustomJobsSystem.OtherThreadInvoker
{
    public class OtherThreadInvoker
    {
        private List<IPoolableJobThread> _tempJobThreadsPool;
        private List<IPoolableJobThread> _infinityJobThreadsPool;
        private readonly Dispatcher.Dispatcher _dispatcher;

        public OtherThreadInvoker(
            CancellationToken mainCancellationToken, 
            Dispatcher.Dispatcher dispatcher, 
            int tempJobsCapacity = 10000,
            int infinityJobCapacity = 1000,
            int tempJobThreadsPoolCount = 6,
            int infinityJobThreadsPoolCount = 3)
        {
            _dispatcher = dispatcher;
            
            InitJobPools(tempJobsCapacity, infinityJobCapacity, tempJobThreadsPoolCount, infinityJobThreadsPoolCount, mainCancellationToken);
        }

        public void Invoke<TInput>(IOtherThreadJob<TInput> job, JobTime jobTime)
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

        private void InitJobPools(
            int tempJobsCapacity,
            int infinityJobCapacity,
            int tempJobThreadsPoolCount,
            int infinityJobThreadsPoolCount,
            CancellationToken cancellationToken)
        {
            _tempJobThreadsPool = new List<IPoolableJobThread>(tempJobThreadsPoolCount);

            for (int i = 0; i < tempJobThreadsPoolCount; i++)
            {
                _tempJobThreadsPool.Add(new TempJobThread(tempJobsCapacity, cancellationToken));
            }
            
            _infinityJobThreadsPool = new List<IPoolableJobThread>(infinityJobThreadsPoolCount);

            for (int i = 0; i < infinityJobThreadsPoolCount; i++)
            {
                _infinityJobThreadsPool.Add(new InfinityJobThread(infinityJobCapacity, cancellationToken));
            }
        }
        
        private void InvokeInfinityJob<TInput>(IOtherThreadJob<TInput> job)
        {
            var leastLoadedThread = GetLeastLoadedThread(_infinityJobThreadsPool);
            leastLoadedThread.JoinJob(() =>
            {
                var output = job.OtherThreadFunc.Invoke();
                _dispatcher.InvokeInFrame(() => job.MainThreadAction?.Invoke(output));
            });
        }

        private void InvokeTempJob<TInput>(IOtherThreadJob<TInput> job)
        {
            var leastLoadedThread = GetLeastLoadedThread(_tempJobThreadsPool);
            leastLoadedThread.JoinJob(() =>
            {
                var output = job.OtherThreadFunc.Invoke();
                _dispatcher.InvokeInFrame(() => job.MainThreadAction?.Invoke(output));
            });
        }

        private IPoolableJobThread GetLeastLoadedThread(List<IPoolableJobThread> pool)
        {
            // var minimumLoadedThread = pool[0];
            //
            // foreach (var tempJobThread in pool)
            // {
            //     if (tempJobThread.LoadedThread < minimumLoadedThread.LoadedThread)
            //     {
            //         minimumLoadedThread = tempJobThread;
            //     }
            // }
            //
            // return minimumLoadedThread;

            return pool[0];
        }
    }
}