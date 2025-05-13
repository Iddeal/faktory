using System;
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
        private const string NUnitPath = nameof(NUnitPath);
        private static int[] _validExitCodes;

        public static void RunTests(string nUnitOptions, string[] inputFiles, int[] validExitCodes = null)
        {
            if (inputFiles == null) throw new ArgumentNullException(nameof(inputFiles));
            _validExitCodes = validExitCodes ?? [];
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
                throw new Exception();
            }

            if (!inputFiles.Any(File.Exists))
            {
                throw new Exception(
                    $"Input file(s) not found: '{string.Join("', '", inputFiles.Where(x => !File.Exists(x)))}'");
            }

            foreach (var path in inputFiles)
            {
                var arguments = $"{nUnitOptions} \"{path}\" ";
                try
                {
                    Process.Run(Config.Get(NUnitPath), arguments, validExitCodes: _validExitCodes);
                }
                catch (InvalidExitCodeException e)
                {
                    FaktoryRunner.ProgressReporter.ReportFailure(e.Message);
                }
            }
        }
    }
}