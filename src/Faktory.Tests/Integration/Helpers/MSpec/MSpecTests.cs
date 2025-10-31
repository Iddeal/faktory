using System;
using System.IO;
using System.Linq;
using Faktory.Core;
using Faktory.Core.Exceptions;
using Faktory.Core.ProgressReporter;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.MSpec
{
    /* NOTE:
     * Due to some bug in R# or NUnit or VS, these tests are flaky until you run them in debug mode
     * first. Then you can run them as usual.
    */
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class MSpecTests
    {
        private string _outputDirectory;
        private static readonly string MSpecNugetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\packages\", "machine.specifications.runner.console");
        private static readonly string TestDummyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\");
        private static readonly string FailingTestsPath = Path.Combine(TestDummyDir, @"FailingTests\bin\debug\", "FailingTests.dll");
        private static readonly string PassingTestsPath = Path.Combine(TestDummyDir, @"PassingTests\bin\debug\", "PassingTests.dll");
        private const string MSpecPath = nameof(MSpecPath);
        static string _mSpecExePath;

        [SetUp]
        public void Init()
        {
            Config.Reset();
            FaktoryRunner.ProgressReporter = new TestProgressReporter();
            Core.Faktory.CurrentActionResult = new ActionResult();
            _outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            Directory.CreateDirectory(_outputDirectory);
            _mSpecExePath = Core.Helpers.MSpec.FindRunner(MSpecNugetDir);
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(_outputDirectory, true);
        }

        [Test, Order(1)]
        [NonParallelizable]
        public void RunTests_CantFindMSpec_ReportsErrorWithHelp()
        {
            Config.Set(MSpecPath, "");
            string mspecOptions = null;
            var assemblies = new string[] { null };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, mspecOptions));

            Assert.That(exception.Message, Is.EqualTo("Config option 'MSpecPath' not set. Please override Configure()."));
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void Run_NoInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(MSpecPath, _mSpecExePath);
            string mspecOptions = null;
            string[] assemblies = null;

            var exception = Assert.Throws<ArgumentNullException>(() => Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, mspecOptions));

            StringAssert.Contains("assemblies", exception.Message);
        }   

        [Test, Order(3)]
        [NonParallelizable]
        public void Run_EmptyAssemblies_ReportsErrorWithHelp()
        {
            Config.Set(MSpecPath, _mSpecExePath);
            string mspecOptions = null;
            var assemblies = Array.Empty<string>();

            var exception = Assert.Throws<Exception>(() => Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, mspecOptions));

            Assert.That(exception.Message, Is.EqualTo("Empty assemblies found."));
        }   

        [Test, Order(4)]
        [NonParallelizable]
        public void Run_CantFindInputFiles_ReportsErrorWithHelp()
        {
            const string nonExistentTestsDll = "non-existent_tests.dll";
            Config.Set(MSpecPath, _mSpecExePath);
            string mspecOptions = null;
            var assemblies = new[] { nonExistentTestsDll };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, mspecOptions));

            Assert.That(exception.Message, Is.EqualTo($"Assemblies not found: '{nonExistentTestsDll}'"));
        }   

        [Test, Order(5)]
        [NonParallelizable]
        public void Run_WithArgs_Succeeds()
        {
            Config.Set(MSpecPath, _mSpecExePath);
            string mspecOptions = null;
            var assemblies = new[] { PassingTestsPath };

            Assert.DoesNotThrow(() => Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, mspecOptions));
        }

        [Test, Order(6)]
        [NonParallelizable]
        public void Run_CompletedTests_RecordsTestsCompletedMessage()
        {
            Config.Set(MSpecPath, _mSpecExePath);
            string mspecOptions = null;
            var assemblies = new[] { PassingTestsPath };

            Assert.DoesNotThrow(() => Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, mspecOptions));
        }

        [Test, Order(7)]
        [NonParallelizable]
        public void Run_ContinueOnFailure_DoesNotThrowOnFaillure()
        {
            Config.Set(MSpecPath, _mSpecExePath);
            var assemblies = new[] { FailingTestsPath };

            Assert.DoesNotThrow(() => Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, continueOnFailedTest: true));
        }

        [Test, Order(8)]
        [NonParallelizable]
        public void Run_WhenToldToNotContinueOnFailure_DoesThrowOnFaillure()
        {
            Config.Set(MSpecPath, _mSpecExePath);
            var assemblies = new[] { FailingTestsPath };

            Assert.Throws<InvalidExitCodeException>(() => Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, continueOnFailedTest: false));
        }

        [Test, Order(9)]
        [NonParallelizable]
        public void Run_WhenTestsPass_RecordsPassingMessageOnTheActionResult()
        {
            Config.Set(MSpecPath, _mSpecExePath);
            var assemblies = new[] { PassingTestsPath };

            Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, continueOnFailedTest: true);

            CollectionAssert.Contains(Core.Faktory.CurrentActionResult.Messages, "MSpec tests completed (Passed)");
        }

        [Test, Order(9)]
        [NonParallelizable]
        public void Run_WhenTestsFail_RecordsFailingMessageOnTheActionResult()
        {
            Config.Set(MSpecPath, _mSpecExePath);
            var assemblies = new[] { FailingTestsPath };

            Core.Helpers.MSpec.RunTests(assemblies, _outputDirectory, continueOnFailedTest: true);

            CollectionAssert.Contains(Core.Faktory.CurrentActionResult.Messages, "MSpec tests completed (Failed)");
        }
    }
}