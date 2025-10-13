using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Faktory.Core.Exceptions;

namespace Faktory.Core.Helpers
{
    /// <summary>
    /// Helpers for working with MSpec.
    /// </summary>
    public static class MSpec
    {
        private const string MSpecPath = nameof(MSpecPath);

        public static void RunTests(string[] assemblies, string outputDirectory, string mspecOptions = "", bool continueOnFailedTest = false)
        {
            ValidateArgs(assemblies);

            var ar = Faktory.CurrentActionResult;
            foreach (var path in assemblies)
            {
                var resultsPath = Path.Combine(outputDirectory, "TestOutput", "MSpecResults.xml");

                if (!Directory.Exists(resultsPath))
                {
                    Directory.CreateDirectory(resultsPath);
                }
                var arguments = $"{mspecOptions} \"{path}\" --xml \"{resultsPath}\" --silent";
                try
                {
                    Process.Run(Config.Get(MSpecPath), arguments, validExitCodes: 0);
                }
                catch (InvalidExitCodeException e)
                {
                    FaktoryRunner.ProgressReporter.ReportFailure(e.Message);
                    ar.LastException = e;
                }
                finally
                {
                    RecordResults(ar, resultsPath);
                }

                if (ar.LastException != null && !continueOnFailedTest) throw ar.LastException;
            }
        }

        private static void RecordResults(ActionResult ar, string resultsPath)
        {
            try
            {
                var assemblies = XDocument.Load(resultsPath)
                    .Root
                    .Elements("assembly");

                var total = 0;
                var failed = 0;
                var passed = 0;
                foreach (var assembly in assemblies)
                {
                    var assemblyIndent = 0;
                    var assemblyName = assembly.Attribute("name").Value;
                    var isAssemblyPrinted = false;
                    var concerns = assembly.Elements("concern");

                    foreach (var concern in concerns)
                    {
                        var concernName = concern.Attribute("name").Value;
                        var concernIndent = string.IsNullOrEmpty(concernName)
                            ? assemblyIndent
                            : assemblyIndent + 1;
                        var isConcernPrinted = false;
                        var contexts = concern.Elements("context");

                        foreach (var context in contexts)
                        {
                            var contextName = context.Attribute("name").Value;
                            var contextIndent = string.IsNullOrEmpty(contextName)
                                ? concernIndent
                                : concernIndent + 1;
                            var isContextPrinted = false;
                            var specifications = context.Elements("specification");

                            foreach (var spec in specifications)
                            {
                                total++;
                                var specName = spec.Attribute("name").Value;
                                var specIndent = contextIndent + 1;
                                var status = spec.Attribute("status").Value;
                                if (status == "passed")
                                {
                                    passed++;
                                    continue;
                                }

                                failed++;
                                if (!isAssemblyPrinted && !string.IsNullOrEmpty(assemblyName))
                                {
                                    ar.AddMessage($"- {assembly.Attribute("name").Value}", assemblyIndent);
                                    isAssemblyPrinted = true;
                                }

                                if (!isConcernPrinted && !string.IsNullOrEmpty(concernName))
                                {
                                    ar.AddMessage($"- {concern.Attribute("name").Value}", concernIndent);
                                    isConcernPrinted = true;
                                }

                                if (!isContextPrinted && !string.IsNullOrEmpty(contextName))
                                {
                                    ar.AddMessage($"- {context.Attribute("name").Value}", contextIndent);
                                    isContextPrinted = true;
                                }

                                ar.AddMessage($"- {specName} ({status})", specIndent);
                                var message = spec
                                    .Element("error")
                                    .Element("message")
                                    .Value
                                    .Replace("\n", $"\n{new string(' ', (specIndent + 1) * ActionResult.IndentWidth )}");
                                ar.AddMessage(message, specIndent + 1);
                            }
                        }
                    }

                    if (failed > 0) ar.AddMessage();
                }

                ar.AddMessage($"MSpec tests completed ({(ar.LastException != null ? "Failed" : "Passed")})");
                ar.AddMessage($"{total} tests");
                ar.AddMessage($"Passed: {passed}", 1);
                ar.AddMessage($"Failed: {failed}", 1);
            }
            catch (Exception e)
            {
                ar.AddMessage();
                ar.AddMessage("##### Error reading results #####");
                ar.AddMessage(e.Message, 1);
                ar.AddMessage(e.StackTrace, 2);
            }
        }

        private static void ValidateArgs(string[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
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
        }
    }
}
