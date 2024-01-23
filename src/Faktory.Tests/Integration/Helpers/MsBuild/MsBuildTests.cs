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
            // Arrange
            Config.Set("MSBuildPath", "");

            // Act - Run the test app
            var result = Core.Helpers.MsBuild.Run("", "nada", "Debug");

            // Assert
            Assert.That(result.Status, Is.EqualTo(Status.Error));
            Assert.That(result.Message, Is.EqualTo("Config option 'MSBuildPath' not set. Please override Configure()."));
            Assert.That(result.ExitCode, Is.Null);
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void Run_WithSolutionOrProjectDoesNotExist_Fails()
        {
            // Arrange
            Config.Set("MSBuildPath", MsBuildPath);

            // Act 
            var result = Core.Helpers.MsBuild.Run("nada");

            // Assert
            Assert.That(result.Status, Is.EqualTo(Status.Error));
            Assert.That(result.Message, Is.EqualTo("Could not find 'nada'"));
            Assert.That(result.ExitCode, Is.Null);
        }   

        [Test, Order(3)]
        [NonParallelizable]
        public void Run_WithDefaults_Succeeds()
        {
            // Arrange
            Config.Set("MSBuildPath", MsBuildPath);
            var solutionPath = Path.Combine(BasePath, "MsBuildTest.sln");

            // Act 
            var result = Core.Helpers.MsBuild.Run(solutionPath);

            // Assert
            Assert.That(result.Status, Is.EqualTo(Status.Ok));
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(0, result.ExitCode);
        }   

        [Test, Order(4)]
        [NonParallelizable]
        public void Run_WithArgs_Succeeds()
        {
            // Arrange 
            Config.Set("MSBuildPath", MsBuildPath);
            var solutionPath = Path.Combine(BasePath, "MsBuildTest.sln");

            // Act 
            var result = Core.Helpers.MsBuild.Run(solutionPath, "Debug","x64", "Build", "/v:quiet");

            // Assert
            Assert.That(result.Status, Is.EqualTo(Status.Ok));
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(0, result.ExitCode);
            Assert.That(File.Exists(Path.Combine(BasePath, @"bin\x64\Debug\MsBuildTest.exe")));
        }
        
        [Test, Order(5)]
        [NonParallelizable]
        public void Run_Clean_Succeeds()
        {
            // Arrange 
            Config.Set("MSBuildPath", MsBuildPath);
            var solutionPath = Path.Combine(BasePath, "MsBuildTest.sln");

            // Act 
            var result = Core.Helpers.MsBuild.Clean(solutionPath);

            // Assert
            Assert.That(result.Status, Is.EqualTo(Status.Ok));
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(0, result.ExitCode);
            Assert.False(File.Exists(Path.Combine(BasePath, @"bin\x64\Debug\MsBuildTest.exe")));
        }
    }
}