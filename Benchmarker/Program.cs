using System.Collections.Generic;

namespace Benchmarker
{
    class Program
    {
        [Benchmark]
        public static void SimpleArrayLoop()
        {
            var arr = new int[1000];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i * i;
            }
        }

        private static List<string> testList;

        [Benchmark]
        public static void Simple()
        {
            int count = 10000000;

            var list = new List<string>();
            for (int i = 0; i < count; i++)
                list.Add("hello");

            testList = list;
        }

        [Benchmark]
        public static void SimpleExpensive()
        {
            int count = 10000;

            var list = new List<string>();

            for (int i = 0; i < count; i++)
            {
                string temp = "";

                for (int j = 0; j < 1000; j++)
                {
                    temp += "a";
                }

                list.Add(temp);
            }

            for (int i = 0; i < count; i++)
                list.Add("hello");

            testList = list;
        }

        static void Main(string[] args)
        {
            BenchmarkRunner.Configuration(5, 5);
            BenchmarkRunner.Run<Program>();
        }
    }
}