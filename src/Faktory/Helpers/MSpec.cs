using System;
using System.IO;
using System.Linq;
using Faktory.Core.Exceptions;

namespace Faktory.Core.Helpers
{
    /// <summary>
    /// Helpers for working with MSpec.
    /// </summary>
    public static class MSpec
    {
        private const string MSpecPath = nameof(MSpecPath);
        private static int[] _validExitCodes;

        public static void RunTests(string[] assemblies, string mspecOptions = "", int[] validExitCodes = null)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            _validExitCodes = validExitCodes ?? [];
            if (string.IsNullOrEmpty(Config.Get(MSpecPath)))
            {
                throw new Exception($"Config option '{MSpecPath}' not set. Please override Configure().");
            }

            if (assemblies.Length == 0)
            {
                throw new Exception("Empty assemblies found.");
            }

            if (!assemblies.Any(File.Exists))
            {
                throw new Exception(
                    $"Assemblies not found: '{string.Join("', '", assemblies.Where(x => !File.Exists(x)))}'");
            }

            foreach (var path in assemblies)
            {
                var arguments = $"{mspecOptions} \"{path}\"";
                try
                {
                    Process.Run(Config.Get(MSpecPath), arguments, validExitCodes: _validExitCodes);
                }
                catch (InvalidExitCodeException e)
                {
                    FaktoryRunner.ProgressReporter.ReportFailure(e.Message);
                }
            }
        }
    }
}
