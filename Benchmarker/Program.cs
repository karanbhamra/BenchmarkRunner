namespace Benchmarker
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Configuration(5, 5);
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}