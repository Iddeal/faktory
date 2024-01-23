using Faktory.Core;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.Process
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class ProcessTests
    {
        [Test, Order(1)]
        public void Run_NotFoundApp_ReturnsFailure()
        {
            // Act 
            var result = Core.Helpers.Process.Run("nada_i_dont_exist.exe");

            // Assert
            Assert.AreEqual(Status.Error, result.Status);
            Assert.AreEqual("The system cannot find the file specified", result.Message);
            Assert.IsNull(result.ExitCode);
        }

        [Test, Order(2)]
        public void Run_TestAppWithNoArguments_ReturnsSuccess()
        {
            // Act - Run the test app
            var result = Core.Helpers.Process.Run("ProcessTest.exe");

            // Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(0, result.ExitCode);
            Assert.AreEqual(Status.Ok, result.Status);
        }

        [Test, Order(3)]
        public void Run_TestAppWithPlannedFailed_ReturnsFailure()
        {
            // Act - Run the test app
            var result = Core.Helpers.Process.Run("ProcessTest.exe", "42");

            // Assert
            Assert.AreEqual(42, result.ExitCode);
            Assert.AreEqual(Status.Error, result.Status);
        }
    }
}