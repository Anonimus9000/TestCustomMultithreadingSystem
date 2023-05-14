using System;
using System.Threading;

namespace MultithreadingTest.CustomJobsSystem.OtherThreadInvoker.JobThreads
{
    public interface IPoolableJobThread
    {
        public Guid ID { get; }
        public int LoadedThread { get; }

        public CancellationToken Token { get; }
        public void JoinJob(Action job);
    }
}