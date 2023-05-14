using System;
using MultithreadingTest.CustomJobsSystem.OtherThreadInvoker.JobThreads;

namespace MultithreadingTest
{
    public struct TestJob : IOtherThreadJob<int>
    {
        public Action<int> MainThreadAction { get; }
        public Func<int> OtherThreadFunc { get; }

        public TestJob(Action<int> mainThreadAction, Func<int> otherThreadFunc)
        {
            MainThreadAction = mainThreadAction;
            OtherThreadFunc = otherThreadFunc;
        }
    }
}