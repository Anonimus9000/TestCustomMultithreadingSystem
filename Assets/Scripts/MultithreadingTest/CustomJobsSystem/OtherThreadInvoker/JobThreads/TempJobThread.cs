using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadingTest.CustomJobsSystem.OtherThreadInvoker.JobThreads
{
    public class TempJobThread : IPoolableJobThread
    {
        private readonly Queue<Action> _jobs;
        public Guid ID { get; }
        public int LoadedThread => _jobs.Count;

        public CancellationToken Token { get; }

        public TempJobThread(int jobsCapacity, CancellationToken token)
        {
            _jobs = new Queue<Action>(jobsCapacity);
            Token = token;
            ID = Guid.NewGuid();
            
            RunTempTimeInvoker();
        }

        public void JoinJob(Action job)
        {
            _jobs.Enqueue(job);
        }
        
        private void RunTempTimeInvoker()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (Token.IsCancellationRequested)
                    {
                        return;
                    }

                    
                    if (_jobs.Count <= 0)
                    {
                        continue;
                    }

                    while (_jobs.Count > 0)
                    {
                        _jobs.Dequeue()?.Invoke();
                    }
                }
            }, Token);
        }
    }
}