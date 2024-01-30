using System;
using System.IO;
using Faktory.Core;
using Faktory.Core.Exceptions;
using Faktory.Core.Logging;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.MsBuild
{

    /* NOTE:
     * Due to some bug in R# or NUnit or VS, these tests are flaky until you run them in debug mode
     * first. Then you can run them as usual.
    */
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class MsBuildTests
    {
        const string BasePath = @"..\..\..\..\MsBuildTest\";
        const string MsBuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\amd64\MSBuild.exe";

        [SetUp]
        public void Init()
        {
            Config.Reset();
        }

        [Test, Order(1)]
        [NonParallelizable]
        public void Run_CantFindMsBuild_ReportsErrorWithHelp()
        {
            Config.Set("MSBuildPath", "");

            var exception = Assert.Throws<Exception>(() => Core.Helpers.MsBuild.Run("", "nada", "Debug"));

            Assert.That(exception.Message, Is.EqualTo("Config option 'MSBuildPath' not set. Please override Configure()."));
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void Run_WithSolutionOrProjectDoesNotExist_Fails()
        {
            Config.Set("MSBuildPath", MsBuildPath);

            var exception = Assert.Throws<Exception>(() => Core.Helpers.MsBuild.Run("nada"));

            Assert.That(exception.Message, Is.EqualTo("Could not find 'nada'"));
        }   

        [Test, Order(3)]
        [NonParallelizable]
        public void Run_WithDefaults_Succeeds()
        {
            Config.Set("MSBuildPath", MsBuildPath);
            var solutionPath = Path.Combine(BasePath, "MsBuildTest.sln");

            Assert.DoesNotThrow(() => Core.Helpers.MsBuild.Run(solutionPath));
        }   

        [Test, Order(4)]
        [NonParallelizable]
        public void Run_WithArgs_Succeeds()
        {
            Config.Set("MSBuildPath", MsBuildPath);
            var solutionPath = Path.Combine(BasePath, "MsBuildTest.sln");

            Assert.DoesNotThrow(() => Core.Helpers.MsBuild.Run(solutionPath, "Debug","x64", args: "/v:quiet"));
            Assert.That(File.Exists(Path.Combine(BasePath, @"bin\x64\Debug\MsBuildTest.exe")));
        }
        
        [Test, Order(5)]
        [NonParallelizable]
        public void Run_Clean_Succeeds()
        {
            Config.Set("MSBuildPath", MsBuildPath);
            var solutionPath = Path.Combine(BasePath, "MsBuildTest.sln");

            Assert.DoesNotThrow(() => Core.Helpers.MsBuild.Clean(solutionPath));
            Assert.False(File.Exists(Path.Combine(BasePath, @"bin\x64\Debug\MsBuildTest.exe")));
        }
    }
}