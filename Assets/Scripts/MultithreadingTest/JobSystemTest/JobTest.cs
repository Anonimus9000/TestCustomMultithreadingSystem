using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace MultithreadingTest.JobSystemTest
{
    [BurstCompile]
    public struct JobTest : IJobParallelFor
    {
        [WriteOnly]
        public NativeArray<int> Ouptut;

        public void Execute(int index)
        {
            // var random = new Random();
            // var result = 0;
            // result += random.Next(0, 100);
            // result -= random.Next(0, 100);
            // Ouptut[index] = result; 

            Ouptut[index] = 10;
        }
    }
}