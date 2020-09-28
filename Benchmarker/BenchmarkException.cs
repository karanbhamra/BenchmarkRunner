using System;

namespace Benchmarker
{
    public class BenchmarkException : Exception
    {
        public BenchmarkException(string msg) : base(msg)
        {
        }

    }
}