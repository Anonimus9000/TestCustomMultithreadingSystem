using System;

namespace MultithreadingTest.CustomJobsSystem.ParallelFor
{
    public interface IParallelJobFor<TInput>
    {
        public Action<TInput> MainThreadAction { get; }
        public Func<TInput> OtherThreadFunc { get; }
    }
}