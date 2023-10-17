using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace Faktory.Tests.Integration.Helpers
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class DeleteTests
    {
        const string BasePath = "./DeleteTestFolder";

        [Test, Order(1)]
        public void DeleteFile_ShouldDeleteFile()
        {
            //Arrange 
            TestHelpers.Disk.CreateFolder(BasePath);
            var file = TestHelpers.Disk.CreateFile(BasePath, "fileToDelete.txt");

            //Act - Delete the file
            var result = global::Faktory.Helpers.Io.DeleteFile(file);

            //Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(Status.Ok, result.Status);
            Assert.IsFalse(File.Exists(file));
        }

        [Test, Order(2)]
        public void Delete_WhenFileInUse_ReportsFileInUse()
        {
            //Arrange 

            //create a file and lock it
            var filePath = TestHelpers.Disk.CreateFile(BasePath, "lockedFile_deleteTest.txt");

            Task.Run(() => TestHelpers.Disk.LockFile(filePath, 3));

            //Act - Delete the file
            var result = global::Faktory.Helpers.Io.DeleteFile(filePath);

            //Assert
            Assert.AreEqual(Status.Error, result.Status);
            Assert.That(result.Message.StartsWith($"Can't delete {filePath}. It's locked by "));
        }
    }
}