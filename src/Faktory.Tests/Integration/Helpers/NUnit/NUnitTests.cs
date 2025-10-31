using System;
using System.IO;
using System.Linq;
using Faktory.Core;
using Faktory.Core.Exceptions;
using Faktory.Core.Logging;
using Faktory.Core.ProgressReporter;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.NUnit
{
    /* NOTE:
     * Due to some bug in R# or NUnit or VS, these tests are flaky until you run them in debug mode
     * first. Then you can run them as usual.
    */
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class NUnitTests
    {
        private string _outputDirectory;
        private static readonly string NUnitNugetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\packages\", "nunit.consolerunner");
        private static readonly string TestDummyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\");
        private static readonly string FailingTestsPath = Path.Combine(TestDummyDir, @"FailingTests\bin\debug\", "FailingTests.dll");
        private static readonly string PassingTestsPath = Path.Combine(TestDummyDir, @"PassingTests\bin\debug\", "PassingTests.dll");
        private const string NUnitPath = nameof(NUnitPath);
        private string _nUnitExePath;

        [SetUp]
        public void Init()
        {
            Config.Reset();
            FaktoryRunner.ProgressReporter = new TestProgressReporter();
            FaktoryRunner.LogWriter = new TestLogWriter();
            Core.Faktory.CurrentActionResult = new ActionResult();
            _outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            Directory.CreateDirectory(_outputDirectory);
            _nUnitExePath = Core.Helpers.NUnit.FindRunner(NUnitNugetDir);
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(_outputDirectory, true);
        }

        [Test, Order(1)]
        [NonParallelizable]
        public void Run_NoInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            string nUnitOptions = null;
            string[] inputFiles = null;

            var exception = Assert.Throws<ArgumentNullException>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, nUnitOptions));

            StringAssert.Contains("inputFiles", exception.Message);
        }   

        [Test, Order(2)]
        [NonParallelizable]
        public void Run_EmptyInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var nUnitOptions = "";
            var inputFiles = Array.Empty<string>();

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, nUnitOptions));

            Assert.That(exception.Message, Is.EqualTo("Empty input file(s) found."));
        }   

        [Test, Order(3)]
        [NonParallelizable]
        public void Run_EmptyStringInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var nUnitOptions = "";
            var inputFiles = new[] { "" };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, nUnitOptions));

            Assert.That(exception.Message, Is.EqualTo("Null or empty input files."));
        }   

        [Test, Order(4)]
        [NonParallelizable]
        public void Run_NullOptions_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            string nUnitOptions = null;
            var inputFiles = new[] { PassingTestsPath };

            var exception = Assert.Throws<ArgumentNullException>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, nUnitOptions));

            StringAssert.Contains("options", exception.Message);
        }   

        [Test, Order(5)]
        [NonParallelizable]
        public void RunTests_CantFindNUnit_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, "");
            var nUnitOptions = "";
            var inputFiles = new string[] { null };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, nUnitOptions));

            Assert.That(exception.Message, Is.EqualTo($"Config option '{NUnitPath}' not set. Please override Configure()."));
        }

        [Test, Order(6)]
        [NonParallelizable]
        public void Run_CantFindInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var nUnitOptions = "";
            var inputFiles = new[] { "non-existent_tests.dll" };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, nUnitOptions));

            Assert.That(exception.Message, Is.EqualTo("Input file(s) not found: 'non-existent_tests.dll'"));
        }   

        [Test, Order(7)]
        [NonParallelizable]
        public void Run_WhenToldToDiscontinueOnFailedTests_PassesCorrectArgToNUnit()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var inputFiles = new[] { FailingTestsPath };

            Assert.Throws<InvalidExitCodeException>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, continueOnFailedTest: false));

            Assert.IsTrue(((TestLogWriter)FaktoryRunner.LogWriter).AllMessages.Any(x => x.StartsWith("Running") && x.Contains("--stoponerror")));
        }

        [Test, Order(8)]
        [NonParallelizable]
        public void Run_WhenToldToDiscontinueOnFailedTestsAndOptionsIncludeArg_DoesNotDuplicateArgPassedToNUnit()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var inputFiles = new[] { FailingTestsPath };

            Assert.Throws<InvalidExitCodeException>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, continueOnFailedTest: false));

            var optionCount = ((TestLogWriter)FaktoryRunner.LogWriter).AllMessages.Single(x => x.StartsWith("Running"))
                .Split(new[] { "--stoponerror" }, StringSplitOptions.None).Length - 1;
            Assert.AreEqual(1, optionCount);
        }

        [Test, Order(9)]
        [NonParallelizable]
        public void Run_WhenToldToDiscontinueOnFailedTests_ThrowsOnFailedTest()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var inputFiles = new[] { FailingTestsPath };

            Assert.Throws<InvalidExitCodeException>(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, continueOnFailedTest: false));
        }

        [Test, Order(10)]
        [NonParallelizable]
        public void Run_WhenToldToContinueOnFailedTests_PassesCorrectArgToNUnit()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var inputFiles = new[] { FailingTestsPath };

            Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, continueOnFailedTest: true);

            Assert.IsFalse(((TestLogWriter)FaktoryRunner.LogWriter).AllMessages.Any(x => x.StartsWith("Running") && x.Contains("--stoponerror")));
        }   

        [Test, Order(11)]
        [NonParallelizable]
        public void Run_WhenToldToContinueOnFailedTests_DoesNotThrowOnFailure()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var inputFiles = new[] { FailingTestsPath };

            Assert.DoesNotThrow(() =>  Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, continueOnFailedTest: true));
        }   

        [Test, Order(12)]
        [NonParallelizable]
        public void Run_WithArgs_Succeeds()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var inputFiles = new[] { PassingTestsPath };

            Assert.DoesNotThrow(() => Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory));
        }

        [Test, Order(13)]
        [NonParallelizable]
        public void Run_WhenTestsPass_RecordsPassingMessageOnTheActionResult()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var inputFiles = new[] { PassingTestsPath };

            Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, continueOnFailedTest: true);

            CollectionAssert.Contains(Core.Faktory.CurrentActionResult.Messages, "NUnit tests completed (Passed)");
        }

        [Test, Order(14)]
        [NonParallelizable]
        public void Run_WhenTestsFail_RecordsFailingMessageOnTheActionResult()
        {
            Config.Set(NUnitPath, _nUnitExePath);
            var inputFiles = new[] { FailingTestsPath };

            Core.Helpers.NUnit.RunTests(inputFiles, _outputDirectory, continueOnFailedTest: true);

            CollectionAssert.Contains(Core.Faktory.CurrentActionResult.Messages, "NUnit tests completed (Failed)");
        }
    }
}