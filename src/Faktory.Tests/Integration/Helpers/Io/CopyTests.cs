using System.IO;
using System.Linq;
using Faktory.Helpers;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class CopyTests
    {
        static string _destination;
        const string BasePath = "./CopyTestFolder";

        [SetUp]
        public void ResetDestination()
        {
            // Delete and recreate the destination folder
            if (Directory.Exists(BasePath))
            {
                Directory.Delete(BasePath, true);
            }

            _destination = Path.Combine(BasePath, "Destination");
            TestHelpers.Disk.CreateFolder(_destination);
        }

        [Test, Order(1)]
        public void Copy_SingleFile_ShouldSucceed()
        {
            // Destination should be empty now
            Assert.IsEmpty(Directory.GetFiles(_destination));
            //Create a dummy file in the BasePath
            var fileName = "file1.txt";
            TestHelpers.Disk.CreateFile(BasePath, fileName);
            var file1 = Path.Combine(BasePath, fileName);
            var copiedFile1 = Path.Combine(_destination, fileName);

            //Act - Copy the file
            var result = Io.Copy(file1, _destination);

            //Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(Status.Ok, result.Status);
            Assert.IsTrue(File.Exists(copiedFile1));
        }

        [Test, Order(2)]
        public void Copy_DestinationDoesNotExist_ShouldSucceed()
        {
            // Destination should be empty now
            Assert.IsEmpty(Directory.GetFiles(_destination));

            //Create a dummy file in the BasePath
            var fileName = "file1.txt";
            TestHelpers.Disk.CreateFile(BasePath, fileName);
            var file1 = Path.Combine(BasePath, fileName);
            var notRealDestination = Path.Combine(_destination, "NotRealFolder");
            var copiedFile1 = Path.Combine(notRealDestination, fileName);

            //Act - Copy the file1 to non-existent directory
            var result = Io.Copy(file1, notRealDestination);

            //Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(Status.Ok, result.Status);
            Assert.True(Directory.Exists(notRealDestination));
            Assert.IsTrue(File.Exists(copiedFile1));
        }

        [Test, Order(3)]
        public void Copy_MultipleFiles_ShouldSucceed()
        {
            // Destination should be empty now
            Assert.IsEmpty(Directory.GetFiles(_destination));

            //Create dummy files in the BasePath
            var files = TestHelpers.Disk.CreateFiles(25, BasePath).ToArray();

            //Act - Copy the files
            var result = Io.Copy(_destination, files);

            //Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(Status.Ok, result.Status);
            Assert.IsTrue(Directory.GetFiles(_destination).Length == 25);
        }
    }
}
