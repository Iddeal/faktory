using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Faktory.Core;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.Io
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class CleanTests
    {
        const string BasePath = "./CleanTestFolder";

        [Test, Order(1)]
        [NonParallelizable]
        public void Clean_ShouldDeleteFilesAndFoldersInDirectory()
        {

            // Arrange 
            TestHelpers.Disk.CreateFoldersWithFiles(BasePath, 5);

            // Act - Clean the path
            Assert.DoesNotThrow(() => Core.Helpers.Io.CleanDirectory(BasePath));

            // Assert
            Assert.IsTrue(Directory.Exists(BasePath));
            Assert.IsEmpty(Directory.GetDirectories(BasePath));
            Assert.IsEmpty(Directory.GetFiles(BasePath));
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void Clean_WhenFileInUse_ReportsFileInUse()
        {
            // Arrange 

            // Create a file and lock it
            TestHelpers.Disk.CreateFile(BasePath, "lockedFile_clean.txt");
            var filePath = Path.Combine(BasePath, "lockedFile_clean.txt");

            Task.Run(() => TestHelpers.Disk.LockFile(filePath, 4));

            // Act - Clean the path
            var exception = Assert.Throws<Exception>(() => Core.Helpers.Io.CleanDirectory(BasePath));

            Thread.Sleep(4000); // Wait for Task.Run to exit
            // Assert
            StringAssert.Contains($"Can't delete `{filePath}`. It's locked by ", exception.Message);
        }
    }
}