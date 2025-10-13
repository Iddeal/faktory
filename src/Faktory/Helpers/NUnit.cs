using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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

        public static void RunTests(string[] assemblies, string outputDirectory, string nUnitOptions = "", bool continueOnFailedTest = false)
        {
            ValidateArgs(assemblies, nUnitOptions);

            if(!continueOnFailedTest && !nUnitOptions.Contains(StopOnError)) nUnitOptions += StopOnError;

            var ar = Faktory.CurrentActionResult;
            var outputDirectoryPath = Path.Combine(outputDirectory, "TestOutput");
            var resultsPath = Path.Combine(outputDirectoryPath, "NUnitResults.xml");

            if (!Directory.Exists(outputDirectoryPath))
            {
                Directory.CreateDirectory(outputDirectoryPath);
            }

            foreach (var path in assemblies)
            {
                var arguments = $"{nUnitOptions} --result={resultsPath};format=nunit3 \"{path}\" ";

                try
                {
                    Process.Run(Config.Get(NUnitPath), arguments, validExitCodes: AllTestsPassedExitCode);
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
                var root = XDocument.Load(resultsPath).Root;
                AddFailedTestMessages(ar, null, root, 0);
                AddSummaryMessages(ar, root);
            }
            catch (Exception e)
            {
                ar.AddMessage("##### Error reading results #####");
                ar.AddMessage(e.Message, 1);
                ar.AddMessage(e.StackTrace, 2);
            }
        }

        private static void AddSummaryMessages(ActionResult ar, XElement root)
        {
            var result = root.Attribute("result").Value;
            var total = root.Attribute("total").Value;
            var passed = root.Attribute("passed").Value;
            var failed = root.Attribute("failed").Value;
            var inconclusive = root.Attribute("inconclusive").Value;
            var skipped = root.Attribute("skipped").Value;

            ar.AddMessage($"NUnit tests completed ({result})");
            ar.AddMessage($"{total} tests");
            ar.AddMessage($"Passed: {passed}", 1);
            ar.AddMessage($"Failed: {failed}", 1);
            ar.AddMessage($"Inconclusive: {inconclusive}", 1);
            ar.AddMessage($"Skipped: {skipped}", 1);
        }

        static void AddFailedTestMessages(ActionResult ar, XElement parent, XElement element, int indent)
        {
            var testCases = element.Elements("test-case").Where(x => x.Attribute("result").Value != "Passed");
            if (testCases.Any())
            {
                var parentName = parent.Attribute("name").Value;
                ar.AddMessage($"- {parentName}", indent);
            }

            foreach (var testCase in testCases)
            {
                var name = testCase.Attribute("name").Value;
                var status = testCase.Attribute("result").Value;
                var message = $"- {name} ({status})";
                ar.AddMessage(message, indent + 1);

                if (status == "Skipped")
                {
                    foreach (var reason in testCase.Elements("reason"))
                    {
                        var reasonMessage = reason.Element("message").Value.Trim()
                            .Replace("\n", $"\n{new string(' ', (indent + 2) * ActionResult.IndentWidth)}");
                        ar.AddMessage(reasonMessage, indent + 2);
                    }
                }
                else
                {
                    var assertions = testCase
                        .Element("assertions")?
                        .Elements("assertion");
                    if (assertions != null)
                    {
                        foreach (var assertion in assertions)
                        {
                            var replaceMessage = assertion.Element("message").Value.Trim()
                                .Replace("\n", $"\n{new string(' ', (indent + 2) * ActionResult.IndentWidth)}");
                            ar.AddMessage(replaceMessage, indent + 2);
                        }
                    }

                    var failure = testCase.Element("failure");
                    if (failure != null)
                    {
                        var failureMessage = failure.Element("message").Value.Trim()
                                .Replace("\n", $"\n{new string(' ', (indent + 2) * ActionResult.IndentWidth)}");
                        ar.AddMessage(failureMessage, indent + 2);
                    }
                }

                ar.AddMessage();
            }

            var testSuiteChildren = element.Elements("test-suite");
            foreach (var testSuiteChild in testSuiteChildren)
            {
                if (testSuiteChild.Attribute("result").Value == "Passed") continue;

                AddFailedTestMessages(ar, element, testSuiteChild, indent);
            }
        }

        private static void ValidateArgs(string[] inputFiles, string nUnitOptions)
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
        }
    }
}