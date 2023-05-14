using System;

namespace MultithreadingTest.CustomJobsSystem.OtherThreadInvoker.JobThreads
{
    public interface IOtherThreadJob<TInput>
    {
        public Action<TInput> MainThreadAction { get; }
        public Func<TInput> OtherThreadFunc { get; }
    }
}