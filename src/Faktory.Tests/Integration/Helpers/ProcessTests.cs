using System.IO;
using NUnit.Framework;
using Faktory.Helpers;

namespace Faktory.Tests.Integration.Helpers
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class ProcessTests
    {
        [Test, Order(1)]
        public void Run_TestAppWithNoArguments_ReturnsSuccess()
        {
            //Act - Run the test app
            var result = Process.Run("ProcessTest.exe");

            //Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(0, result.ExitCode);
            Assert.AreEqual(Status.Ok, result.Status);
        }

        [Test, Order(2)]
        public void Run_TestAppWithPlannedFailed_ReturnsFailure()
        {
            //Act - Run the test app
            var result = Process.Run("ProcessTest.exe", "42");

            //Assert
            Assert.AreEqual(42, result.ExitCode);
            Assert.AreEqual(Status.Error, result.Status);
        }
    }
}