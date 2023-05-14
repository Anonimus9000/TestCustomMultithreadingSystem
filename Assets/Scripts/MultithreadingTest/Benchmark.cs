using System.Diagnostics;
using MultithreadingTest.CustomJobsSystem.Dispatcher;
using MultithreadingTest.CustomJobsSystem.OtherThreadInvoker;
using MultithreadingTest.CustomJobsSystem.OtherThreadInvoker.JobThreads;
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
        private IParallelJobFor<int>[] _parallelJobsFor;
        private IOtherThreadJob<int>[] _otherThreadJobs;

        private void Awake()
        {
            InitializeJobs();
        }

        private void Start()
        {
            _dispatcher = new GameObject().AddComponent<Dispatcher>();
            _dispatcher.Initialize(IterationCount);
            _parallelInvokerFor = new ParallelInvokerFor(_dispatcher, IterationCount);
            _otherThreadInvoker =
                new OtherThreadInvoker(ApplicationThreadingSynchronizer.QuitToken, _dispatcher, IterationCount);
            
            var stopwatch = new Stopwatch();
            
            //yield return new WaitForSeconds(1);

            stopwatch.Start();
            //ParallelForBench();
            stopwatch.Stop();
            _parallelForResult.text = stopwatch.ElapsedMilliseconds.ToString();

            //yield return new WaitForSeconds(1);
            
            stopwatch.Reset();
            stopwatch.Start();
            OtherThreadInvokerBench();
            stopwatch.Stop();
            Debug.Break();
            _otherThreadResult.text = stopwatch.ElapsedMilliseconds.ToString();

            //yield return new WaitForSeconds(1);

            stopwatch.Reset();
            stopwatch.Start();
            //MainThreadInvoker();
            stopwatch.Stop();
            _mainThreadResult.text = stopwatch.ElapsedMilliseconds.ToString();
            
            //yield return new WaitForSeconds(1);

            stopwatch.Reset();
            stopwatch.Start();
            //JobSystemBench();
            stopwatch.Stop();
            _jobSystemResult.text = stopwatch.ElapsedMilliseconds.ToString();
        }

        private void InitializeJobs()
        {
            var random = new Random();
            _parallelJobsFor = new IParallelJobFor<int>[IterationCount];
            for (var i = 0; i < IterationCount; i++)
            {
                _parallelJobsFor[i] = new TestJobFor(
                    _ =>
                    {
                    },
                    () =>
                    {
                        var result = 0;
                        result += random.Next(0, 100);
                        result -= random.Next(0, 100);
                        return result;
                    });
            }
            
            _otherThreadJobs = new IOtherThreadJob<int>[IterationCount];
            for (var i = 0; i < IterationCount; i++)
            {
                _otherThreadJobs[i] = new TestJob(
                    _ =>
                    {
                    },
                    () =>
                    {
                        var result = 0;
                        result += random.Next(0, 100);
                        result -= random.Next(0, 100);
                        return result;
                    });
            }
        }

        private void MainThreadInvoker()
        {
            for (var i = 0; i < IterationCount; i++)
            {
                var job = _parallelJobsFor[i];
                var output = job.OtherThreadFunc.Invoke();
                job.MainThreadAction.Invoke(output);
            }
        }

        private void ParallelForBench()
        {
            _parallelInvokerFor.Invoke(_parallelJobsFor, ParallelJobTime.Frame);
        }

        private void OtherThreadInvokerBench()
        {
            for (var i = 0; i < IterationCount; i++)
            {
                _otherThreadInvoker.Invoke(_otherThreadJobs[i], JobTime.TempTime);
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