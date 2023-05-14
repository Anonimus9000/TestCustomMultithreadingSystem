using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadingTest.CustomJobsSystem.OtherThreadInvoker.JobThreads
{
    public class InfinityJobThread : IPoolableJobThread
    {
        public Guid ID { get; }
        public int LoadedThread { get; }
        public CancellationToken Token { get; }

        private readonly List<Action> _jobs;

        public InfinityJobThread(int jobsCapacity, CancellationToken token)
        {
            _jobs = new List<Action>(jobsCapacity);
            Token = token;
            ID = Guid.NewGuid();
            
            RunInfinityInvoker();
        }
        
        public void JoinJob(Action job)
        {
            _jobs.Add(job);
        }

        private void RunInfinityInvoker()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (Token.IsCancellationRequested)
                    {
                        return;
                    }

                    foreach (var action in _jobs)
                    {
                        action?.Invoke();
                    }
                }
            }, Token);
        }
    }
}