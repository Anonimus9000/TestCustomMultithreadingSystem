using System;
using System.Diagnostics;
using MultithreadingTest.CustomJobsSystem.Dispatcher;
using MultithreadingTest.CustomJobsSystem.OtherThreadInvoker;
using MultithreadingTest.CustomJobsSystem.ParallelFor;
using MultithreadingTest.JobSystemTest;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace MultithreadingTest
{
    public class Benchmark : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _parallelForResult;
        
        [SerializeField]
        private TMP_Text _otherThreadResult;
        
        [SerializeField]
        private TMP_Text _mainThreadResult;
        
        [SerializeField]
        private TMP_Text _jobSystemResult;
        
        private const int IterationCount = 1000000;
        
        private Dispatcher _dispatcher;
        private ParallelInvokerFor _parallelInvokerFor;
        private OtherThreadInvoker _otherThreadInvoker;
        private IParallelJobFor<int>[] _jobs;

        private void Awake()
        {
            var stopwatch = new Stopwatch();
            _dispatcher = new GameObject().AddComponent<Dispatcher>();
            _dispatcher.Initialize(IterationCount);
            _parallelInvokerFor = new ParallelInvokerFor(_dispatcher, IterationCount);
            _otherThreadInvoker =
                new OtherThreadInvoker(ApplicationThreadingSynchronizer.QuitToken, _dispatcher, IterationCount);
            InitializeJobs();
           
            stopwatch.Start();
            ParallelForBench();
            stopwatch.Stop();
            _parallelForResult.text = stopwatch.ElapsedMilliseconds.ToString();
            
            stopwatch.Start();
            OtherThreadInvokerBench();
            stopwatch.Stop();
            _otherThreadResult.text = stopwatch.ElapsedMilliseconds.ToString();

            
            stopwatch.Start();
            MainThreadInvoker();
            stopwatch.Stop();
            _mainThreadResult.text = stopwatch.ElapsedMilliseconds.ToString();
            
            stopwatch.Start();
            JobSystemBench();
            stopwatch.Stop();
            _jobSystemResult.text = stopwatch.ElapsedMilliseconds.ToString();
        }

        private void InitializeJobs()
        {
            Random random = new Random();
            _jobs = new IParallelJobFor<int>[IterationCount];
            for (int i = 0; i < IterationCount; i++)
            {
                _jobs[i] = new TestJobFor(
                    result =>
                    {
                    },
                    () =>
                    {
                        int result = 0;
                        result += random.Next(0, 100);
                        result -= random.Next(0, 100);
                        return result;
                    });
            }
        }

        private void MainThreadInvoker()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                var job = _jobs[i];
                var output = job.OtherThreadFunc.Invoke();
                job.MainThreadAction.Invoke(output);
            }
        }

        private void ParallelForBench()
        {
            _parallelInvokerFor.Invoke(_jobs, ParallelJobTime.Frame);
        }

        private void OtherThreadInvokerBench()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                _otherThreadInvoker.Invoke(_jobs[i], JobTime.TempTime);
            }
        }

        private void JobSystemBench()
        {
            var output = new NativeArray<int>(IterationCount, Allocator.Persistent);

            var job = new JobTest
            {
                Ouptut = output,
            };
            
            var jobHandle = job.Schedule(IterationCount, 16);
            
            jobHandle.Complete();

            output.Dispose();
        }
    }
}