using System;
using System.IO;
using System.Linq;
using Faktory.Core;
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
        private const string NUnitPath = nameof(NUnitPath);
        private static readonly string NUnitExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Lib", "NUnit.ConsoleRunner", "nunit3-console.exe");

        [SetUp]
        public void Init()
        {
            Config.Reset();
            FaktoryRunner.ProgressReporter = new TestProgressReporter();
            FaktoryRunner.LogWriter = new TestLogWriter();
        }

        [Test, Order(1)]
        [NonParallelizable]
        public void Run_NoInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, NUnitExePath);
            string nUnitOptions = null;
            string[] inputFiles = null;

            var exception = Assert.Throws<ArgumentNullException>(() => Core.Helpers.NUnit.RunTests(inputFiles, nUnitOptions));

            StringAssert.Contains("inputFiles", exception.Message);
        }   

        [Test, Order(2)]
        [NonParallelizable]
        public void Run_EmptyInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, NUnitExePath);
            var nUnitOptions = "";
            var inputFiles = Array.Empty<string>();

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(inputFiles, nUnitOptions));

            Assert.That(exception.Message, Is.EqualTo("Empty input file(s) found."));
        }   

        [Test, Order(3)]
        [NonParallelizable]
        public void Run_EmptyStringInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, NUnitExePath);
            var nUnitOptions = "";
            var inputFiles = new[] { "" };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(inputFiles, nUnitOptions));

            Assert.That(exception.Message, Is.EqualTo("Null or empty input files."));
        }   

        [Test, Order(4)]
        [NonParallelizable]
        public void Run_NullOptions_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, NUnitExePath);
            string nUnitOptions = null;
            var inputFiles = new[] { "non-existent_tests.dll" };

            var exception = Assert.Throws<ArgumentNullException>(() => Core.Helpers.NUnit.RunTests(inputFiles, nUnitOptions));

            StringAssert.Contains("nUnitOptions", exception.Message);
        }   

        [Test, Order(5)]
        [NonParallelizable]
        public void RunTests_CantFindNUnit_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, "");
            var nUnitOptions = "";
            var inputFiles = new string[] { null };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(inputFiles, nUnitOptions));

            Assert.That(exception.Message, Is.EqualTo($"Config option '{NUnitPath}' not set. Please override Configure()."));
        }

        [Test, Order(6)]
        [NonParallelizable]
        public void Run_CantFindInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, NUnitExePath);
            var nUnitOptions = "";
            var inputFiles = new[] { "non-existent_tests.dll" };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(inputFiles, nUnitOptions));

            Assert.That(exception.Message, Is.EqualTo("Input file(s) not found: 'non-existent_tests.dll'"));
        }   

        [Test, Order(7)]
        [NonParallelizable]
        public void Run_WhenToldToDiscontinueOnFailedTests_PassesCorrectArgToNUnit()
        {
            Config.Set(NUnitPath, NUnitExePath);
            var nUnitOptions = "";
            var inputFiles = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDummy.dll") };

            Assert.DoesNotThrow(() => Core.Helpers.NUnit.RunTests(inputFiles, continueOnFailedTest: false));
            Assert.IsTrue(((TestLogWriter)FaktoryRunner.LogWriter).AllMessages.Any(x => x.StartsWith("Running") && x.Contains("--stoponerror")));
        }   

        [Test, Order(8)]
        [NonParallelizable]
        public void Run_WhenToldToDiscontinueOnFailedTestsAndOptionsIncludeArg_DoesNotDuplicateArgPassedToNUnit()
        {
            Config.Set(NUnitPath, NUnitExePath);
            var nUnitOptions = "--stoponerror";
            var inputFiles = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDummy.dll") };

            Assert.DoesNotThrow(() => Core.Helpers.NUnit.RunTests(inputFiles, continueOnFailedTest: false));
            var optionCount = ((TestLogWriter)FaktoryRunner.LogWriter).AllMessages.Single(x => x.StartsWith("Running")).Split(new[] { "--stoponerror" }, StringSplitOptions.None).Length - 1;
            Assert.AreEqual(1, optionCount);
        }

        [Test, Order(9)]
        [NonParallelizable]
        public void Run_WhenToldToContinueOnFailedTests_PassesCorrectArgToNUnit()
        {
            Config.Set(NUnitPath, NUnitExePath);
            var nUnitOptions = "";
            var inputFiles = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDummy.dll") };

            Assert.DoesNotThrow(() => Core.Helpers.NUnit.RunTests(inputFiles, continueOnFailedTest: true));
            Assert.IsFalse(((TestLogWriter)FaktoryRunner.LogWriter).AllMessages.Any(x => x.StartsWith("Running") && x.Contains("--stoponerror")));
        }   

        [Test, Order(10)]
        [NonParallelizable]
        public void Run_WithArgs_Succeeds()
        {
            Config.Set(NUnitPath, NUnitExePath);
            var nUnitOptions = "";
            var inputFiles = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDummy.dll") };

            Assert.DoesNotThrow(() => Core.Helpers.NUnit.RunTests(inputFiles));
        }
    }
}