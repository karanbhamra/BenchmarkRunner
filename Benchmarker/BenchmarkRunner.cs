using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Benchmarker
{
    public static class BenchmarkRunner
    {
        private static bool _settingsSet = false;
        private static int _warmupRunCount;
        private static int _benchmarkRunCount;

        public static void Configuration(int warmupCount = 3, int benchmarkCount = 3)
        {
            _warmupRunCount = warmupCount;
            _benchmarkRunCount = benchmarkCount;
            _settingsSet = true;
        }

        public static void Run<T>()
        {
            if (!_settingsSet)
                Configuration();

            var methods = DiscoverTests(typeof(T));

            if (methods.Count == 0)
            {
                throw new BenchmarkException("No benchmarks found.");
            }

            foreach (var method in methods)
            {
                Console.WriteLine($"Benchmarking method: {method.Name}");

                try
                {
                    WarmupMethod(method);
                }
                catch (TargetInvocationException ex)
                {
                    var innerException = ex.InnerException;

                    Console.WriteLine($"Failed method: {method.Name}, Exception: {innerException?.Message}");
                }

                // Give the test as good a chance as possible
                // of avoiding garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var runTimes = new List<double>();

                for (int i = 0; i < _benchmarkRunCount; i++)
                {
                    var stopWatch = Stopwatch.StartNew();
                    method.Invoke(null, null);
                    stopWatch.Stop();
#if DEBUG
                    Console.WriteLine($"Run #{i + 1}, Time(ms): {stopWatch.Elapsed.TotalMilliseconds}");
#endif
                    runTimes.Add(stopWatch.Elapsed.TotalMilliseconds);
                }

                Console.WriteLine($"Method: {method.Name}, Average Time (ms): {runTimes.Average()}");
            }
        }

        private static void WarmupMethod(MethodInfo method)
        {
            for (int i = 0; i < _warmupRunCount; i++)
            {
#if DEBUG
                Console.WriteLine($"Warmup Run #{i + 1}");
#endif
                method.Invoke(null, null);
            }
        }

        private static IList<MethodInfo> DiscoverTests(Type benchmarkClass)
        {
            var methods = new List<MethodInfo>();

            var benchmarkMethods = benchmarkClass.GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach (var method in benchmarkMethods)
            {
                if (method.GetParameters().Length == 0)
                {
                    foreach (var attribute in method.GetCustomAttributes())
                    {
                        if (attribute is BenchmarkAttribute)
                        {
                            methods.Add(method);
                        }
                    }
                }
            }

            return methods;
        }
    }
}