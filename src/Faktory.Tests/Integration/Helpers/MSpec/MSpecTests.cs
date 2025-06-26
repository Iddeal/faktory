using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Faktory.Core;
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
        private const string MSpecPath = nameof(MSpecPath);
        private static readonly string MSpecExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Lib", "Machine.Specifications.Runner.Console", "mspec-clr4.exe");

        [SetUp]
        public void Init()
        {
            Config.Reset();
            FaktoryRunner.ProgressReporter = new TestProgressReporter();
        }

        [Test, Order(1)]
        [NonParallelizable]
        public void RunTests_CantFindMSpec_ReportsErrorWithHelp()
        {
            Config.Set(MSpecPath, "");
            string mspecOptions = null;
            var assemblies = new string[] { null };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.MSpec.RunTests(assemblies, mspecOptions));

            Assert.That(exception.Message, Is.EqualTo("Config option 'MSpecPath' not set. Please override Configure()."));
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void Run_NoInputFiles_ReportsErrorWithHelp()
        {
            Config.Set(MSpecPath, MSpecExePath);
            string mspecOptions = null;
            string[] assemblies = null;

            var exception = Assert.Throws<ArgumentNullException>(() => Core.Helpers.MSpec.RunTests(assemblies, mspecOptions));

            StringAssert.Contains("assemblies", exception.Message);
        }   

        [Test, Order(3)]
        [NonParallelizable]
        public void Run_EmptyAssemblies_ReportsErrorWithHelp()
        {
            Config.Set(MSpecPath, MSpecExePath);
            string mspecOptions = null;
            var assemblies = Array.Empty<string>();

            var exception = Assert.Throws<Exception>(() => Core.Helpers.MSpec.RunTests(assemblies, mspecOptions));

            Assert.That(exception.Message, Is.EqualTo("Empty assemblies found."));
        }   

        [Test, Order(4)]
        [NonParallelizable]
        public void Run_CantFindInputFiles_ReportsErrorWithHelp()
        {
            const string nonExistentTestsDll = "non-existent_tests.dll";
            Config.Set(MSpecPath, MSpecExePath);
            string mspecOptions = null;
            var assemblies = new[] { nonExistentTestsDll };

            var exception = Assert.Throws<Exception>(() => Core.Helpers.MSpec.RunTests(assemblies, mspecOptions));

            Assert.That(exception.Message, Is.EqualTo($"Assemblies not found: '{nonExistentTestsDll}'"));
        }   

        [Test, Order(5)]
        [NonParallelizable]
        public void Run_WithArgs_Succeeds()
        {
            Config.Set(MSpecPath, MSpecExePath);
            string mspecOptions = null;
            var assemblies = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDummy.dll") };
            Assert.DoesNotThrow(() => Core.Helpers.MSpec.RunTests(assemblies, mspecOptions, new []{ 0 }));
        }

        [Test, Order(6)]
        [NonParallelizable]
        public void Run_InvalidExitCode_ReportsFailure()
        {
            Config.Set(MSpecPath, "ProcessTest.exe");
            const int exitCode = int.MinValue;
            var mspecOptions = exitCode.ToString();
            var assemblies = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDummy.dll") };

            Assert.DoesNotThrow(() => Core.Helpers.MSpec.RunTests(assemblies, mspecOptions));

            Assert.True(((TestProgressReporter)FaktoryRunner.ProgressReporter).AllMessages.Any(x => x.Contains(exitCode.ToString())));
        }
    }
}