using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class CleanTests
    {
        const string BasePath = "./CleanTestFolder";

        [Test, Order(1)]
        public void Clean_ShouldDeleteFilesAndFoldersInDirectory()
        {

            // Arrange 
            TestHelpers.Disk.CreateFoldersWithFiles(BasePath, 5);

            // Act - Clean the path
            var result = global::Faktory.Helpers.Io.CleanDirectory(BasePath);

            // Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(Status.Ok, result.Status);
            Assert.IsTrue(Directory.Exists(BasePath));
            Assert.IsEmpty(Directory.GetDirectories(BasePath));
            Assert.IsEmpty(Directory.GetFiles(BasePath));
        }

        [Test, Order(2)]
        public void Clean_WhenFileInUse_ReportsFileInUse()
        { 
            // Arrange 

            // Create a file and lock it
            TestHelpers.Disk.CreateFile(BasePath, "lockedFile_clean.txt");
            var filePath = Path.Combine(BasePath, "lockedFile_clean.txt");

            Task.Run(() => TestHelpers.Disk.LockFile(filePath, 3));

            // Act - Clean the path
            var result = global::Faktory.Helpers.Io.CleanDirectory(BasePath);

            // Assert
            Assert.AreEqual(Status.Error, result.Status);
            Assert.That(result.Message.StartsWith($"Can't delete `{filePath}`. It's locked by "));
        }
    }
}