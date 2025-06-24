using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Faktory.Core.Exceptions;

namespace Faktory.Core.Helpers
{
    /// <summary>
    /// Helpers for working with NUnit.
    /// </summary>
    public static class NUnit
    {
        private const int AllTestsPassedExitCode = 0;
        private const string NUnitPath = nameof(NUnitPath);
        private const string StopOnError = " --stoponerror";

        public static void RunTests(string[] inputFiles, string nUnitOptions = "", bool continueOnFailedTest = true)
        {
            if (inputFiles == null) throw new ArgumentNullException(nameof(inputFiles));
            if (nUnitOptions == null) throw new ArgumentNullException(nameof(nUnitOptions));
            if (string.IsNullOrEmpty(Config.Get(NUnitPath)))
            {
                throw new Exception($"Config option '{NUnitPath}' not set. Please override Configure().");
            }

            if (inputFiles.Length == 0)
            {
                throw new Exception("Empty input file(s) found.");
            }

            if (inputFiles.Any(string.IsNullOrEmpty))
            {
                throw new Exception("Null or empty input files.");
            }

            var nonExistentFiles = inputFiles.Where(x => !File.Exists(x)).ToList();
            if (nonExistentFiles.Any())
            {
                throw new Exception(
                    $"Input file(s) not found: '{string.Join("', '", nonExistentFiles)}'");
            }

            var validExitCodes = new List<int> { AllTestsPassedExitCode };
            if (continueOnFailedTest)
            {
                validExitCodes.AddRange(FailedTestCounts());
            }
            else
            {
                if(!nUnitOptions.Contains(StopOnError)) nUnitOptions += StopOnError;
            }

            foreach (var path in inputFiles)
            {
                var arguments = $"{nUnitOptions} \"{path}\" ";

                try
                {
                    Process.Run(Config.Get(NUnitPath), arguments, validExitCodes: validExitCodes.ToArray());
                }
                catch (InvalidExitCodeException e)
                {
                    FaktoryRunner.ProgressReporter.ReportFailure(e.Message);
                }
            }

        }

        private static int[] FailedTestCounts() => Enumerable.Range(1, 100).Select(i => i).ToArray();
    }
}