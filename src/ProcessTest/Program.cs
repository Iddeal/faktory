using System;
using System.Threading;

namespace ProcessTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Faktory Process Test");
            Console.WriteLine("This exists to help test Faktory.Helpers.Process");
            Console.WriteLine("Pass an integer as an argument and it'll be used as the exit code.");
            Console.WriteLine("Otherwise, the ExitCode will be zero.");
            Console.WriteLine("This process runs and sleeps for 1 second before exiting.");

            Thread.Sleep(1000);

            try
            {
                if (args.Length <= 0) return;

                var exitCode = int.Parse(args[0]);
                Console.Error.WriteLine($"Setting exit code to {exitCode}");
                Environment.ExitCode = exitCode;
            }
            catch
            {
                Console.WriteLine("Invalid parameter sent.");
                Environment.ExitCode = 42;
            }
        }
    }
}
