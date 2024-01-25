using System;
using System.IO;
using System.Threading.Tasks;
using Faktory.Core;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.Io
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class DeleteTests
    {
        const string BasePath = "./DeleteTestFolder";

        [Test, Order(1)]
        [NonParallelizable]
        public void DeleteFile_ShouldDeleteFile()
        {
            // Arrange 
            TestHelpers.Disk.CreateFolder(BasePath);
            var file = TestHelpers.Disk.CreateFile(BasePath, "fileToDelete.txt");

            // Act - Delete the file
            Assert.DoesNotThrow(() => Core.Helpers.Io.DeleteFile(file));

            // Assert
            Assert.IsFalse(File.Exists(file));
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void DeleteFile_WhenFileInUse_ReportsFileInUse()
        {
            // Arrange 

            // Create a file and lock it
            var filePath = TestHelpers.Disk.CreateFile(BasePath, "lockedFile_deleteTest.txt");

            Task.Run(() => TestHelpers.Disk.LockFile(filePath, 3));

            // Act - Delete the file
            var exception = Assert.Throws<Exception>(() => Core.Helpers.Io.DeleteFile(filePath));

            // Assert
            StringAssert.Contains($"Can't delete `{filePath}`. It's locked by ", exception.Message);
        }

        [Test, Order(3)]
        [NonParallelizable]
        public void DeleteFile_MultipleFiles()
        {
            // Arrange 
            // Create multiple files
            var files = TestHelpers.Disk.CreateFiles(5, BasePath);

            // Act - Delete the files
            Assert.DoesNotThrow(() => Core.Helpers.Io.DeleteFiles(files));

            // Assert
            foreach (var file in files)
            {
                Assert.IsFalse(File.Exists(file));
            }
        }

        [Test, Order(4)]
        [NonParallelizable]
        public void DeleteDirectory_WhenEmpty_Succeeds()
        {
            // Arrange 
            // Create an empty directory
            var directory = TestHelpers.Disk.CreateFolder(Path.Combine(BasePath, "DeleteDirTestFolder"));

            // Act - Delete the directory
            Assert.DoesNotThrow(() => Core.Helpers.Io.DeleteDirectory(directory));

            // Assert
            Assert.IsFalse(Directory.Exists(directory));
        }

        [Test, Order(5)]
        [NonParallelizable]
        public void DeleteDirectory_WhenNotEmpty_Succeeds()
        {
            // Arrange 
            // Create a directory with files
            var directory = Path.Combine(BasePath, "DeleteDirTestFolder");
            var files = TestHelpers.Disk.CreateFolderWithFiles(directory, 4);

            // Act - Delete the directory
            Assert.DoesNotThrow(() => Core.Helpers.Io.DeleteDirectory(directory));

            // Assert
            Assert.IsFalse(Directory.Exists(directory));
            foreach (var file in files)
            {
                Assert.IsFalse(File.Exists(file));
            }
        }
    }
}