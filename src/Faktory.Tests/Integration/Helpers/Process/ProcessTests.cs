using System;
using Faktory.Core;
using Faktory.Core.Exceptions;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.Process
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class ProcessTests
    {
        [Test, Order(1)]
        [NonParallelizable]
        public void Run_NotFoundApp_ReturnsFailure()
        {
            // Act 
            var exception = Assert.Throws<Exception>(() => Core.Helpers.Process.Run("nada_i_dont_exist.exe"));

            // Assert
            StringAssert.Contains("The system cannot find the file specified", exception?.Message);
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void Run_TestAppWithNoArguments_ReturnsSuccess()
        {
            // Act - Run the test app
            Assert.DoesNotThrow(() => Core.Helpers.Process.Run("ProcessTest.exe"));
        }

        [Test, Order(3)]
        [NonParallelizable]
        public void Run_TestAppWithPlannedFailed_ReturnsFailure()
        {
            // Act - Run the test app
            Assert.Throws<InvalidExitCodeException>(() => Core.Helpers.Process.Run("ProcessTest.exe", "42"));
        }
    }
}