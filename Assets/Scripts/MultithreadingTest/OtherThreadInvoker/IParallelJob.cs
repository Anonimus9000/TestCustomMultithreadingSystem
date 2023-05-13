using System;

namespace MultithreadingTest.OtherThreadInvoker
{
    public interface IParallelJob<TInput>
    {
        public Action<TInput> MainThreadAction { get; }
        public Func<TInput> OtherThreadFunc { get; }
    }
}