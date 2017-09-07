using System;
using BenchmarkDotNet.Running;
using ConsoleApp1;

namespace BenchmarkTest
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CreatorTest>();

            Console.WriteLine(summary);

            Console.ReadLine();
        }
    }
}