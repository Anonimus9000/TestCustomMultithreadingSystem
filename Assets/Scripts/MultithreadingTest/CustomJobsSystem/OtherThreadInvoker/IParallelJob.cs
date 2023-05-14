using System;

namespace MultithreadingTest.CustomJobsSystem.OtherThreadInvoker
{
    public interface IParallelJob<TInput>
    {
        public Action<TInput> MainThreadAction { get; }
        public Func<TInput> OtherThreadFunc { get; }
    }
}