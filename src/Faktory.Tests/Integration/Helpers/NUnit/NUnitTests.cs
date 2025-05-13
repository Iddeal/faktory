using System;
using System.IO;
using System.Linq;
using Faktory.Core;
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
        }

        [Test, Order(1)]
        [NonParallelizable]
        public void RunTests_CantFindNUnit_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, "");
            IProgressReporter progressReporter = new TestProgressReporter();
            string nUnitOptions = null;
            var inputFiles = new string[] { null };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(nUnitOptions, inputFiles));

            Assert.That(exception.Message, Is.EqualTo($"Config option '{NUnitPath}' not set. Please override Configure()."));
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void Run_NoInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, NUnitExePath);
            IProgressReporter progressReporter = new TestProgressReporter();
            string nUnitOptions = null;
            string[] inputFiles = null;

            var exception = Assert.Throws<ArgumentNullException>(() => Core.Helpers.NUnit.RunTests(nUnitOptions, inputFiles));

            StringAssert.Contains("inputFiles", exception.Message);
        }   

        [Test, Order(3)]
        [NonParallelizable]
        public void Run_EmptyInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, NUnitExePath);
            IProgressReporter progressReporter = new TestProgressReporter();
            string nUnitOptions = null;
            var inputFiles = Array.Empty<string>();

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(nUnitOptions, inputFiles));

            Assert.That(exception.Message, Is.EqualTo("Empty input file(s) found."));
        }   

        [Test, Order(4)]
        [NonParallelizable]
        public void Run_CantFindInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(NUnitPath, NUnitExePath);
            IProgressReporter progressReporter = new TestProgressReporter();
            string nUnitOptions = null;
            var inputFiles = new[] { "non-existent_tests.dll" };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.NUnit.RunTests(nUnitOptions, inputFiles));

            Assert.That(exception.Message, Is.EqualTo("Input file(s) not found: 'non-existent_tests.dll'"));
        }   

        [Test, Order(5)]
        [NonParallelizable]
        public void Run_WithArgs_Succeeds()
        {
            Config.Set(NUnitPath, NUnitExePath);
            IProgressReporter progressReporter = new TestProgressReporter();
            string nUnitOptions = null;
            var inputFiles = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDummy.dll") };

            Assert.DoesNotThrow(() => Core.Helpers.NUnit.RunTests(null, inputFiles));
        }

        [Test, Order(6)]
        [NonParallelizable]
        public void Run_InvalidExitCode_ReportsFailure()
        {
            Config.Set(NUnitPath, "ProcessTest.exe");
            var progressReporter = new TestProgressReporter();
            const int exitCode = int.MinValue;
            var nUnitOptions = exitCode.ToString();
            var inputFiles = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDummy.dll") };

            Assert.DoesNotThrow(() => Core.Helpers.NUnit.RunTests(nUnitOptions, inputFiles));

            Assert.True(((TestProgressReporter)FaktoryRunner.ProgressReporter).AllMessages.Any(x => x.Contains(exitCode.ToString())));
        }
    }
}