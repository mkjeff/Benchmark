using System;
using BenchmarkDotNet.Running;

namespace SequentialGuid
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SequentialGuidBenchmark>();

            Console.WriteLine(summary);

            Console.ReadLine();
        }
    }
}
