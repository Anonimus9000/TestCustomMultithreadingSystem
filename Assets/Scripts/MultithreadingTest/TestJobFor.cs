using System;
using MultithreadingTest.ParallelFor;

namespace MultithreadingTest
{
    public struct TestJobFor : IParallelJobFor<int>
    {
        public Action<int> MainThreadAction { get; }
        public Func<int> OtherThreadFunc { get; }

        public TestJobFor(Action<int> mainThreadAction, Func<int> otherThreadFunc)
        {
            MainThreadAction = mainThreadAction;
            OtherThreadFunc = otherThreadFunc;
        }
    }
}