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
            // Create a dummy file in the BasePath
            var fileName = "file1.txt";
            TestHelpers.Disk.CreateFile(BasePath, fileName);
            var file1 = Path.Combine(BasePath, fileName);
            var copiedFile1 = Path.Combine(_destination, fileName);

            // Act - Copy the file
            var result = Io.Copy(file1, _destination);

            // Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(Status.Ok, result.Status);
            Assert.IsTrue(File.Exists(copiedFile1));
        }

        [Test, Order(2)]
        public void Copy_DestinationDoesNotExist_ShouldSucceed()
        {
            // Destination should be empty now
            Assert.IsEmpty(Directory.GetFiles(_destination));

            // Create a dummy file in the BasePath
            var fileName = "file1.txt";
            TestHelpers.Disk.CreateFile(BasePath, fileName);
            var file1 = Path.Combine(BasePath, fileName);
            var notRealDestination = Path.Combine(_destination, "NotRealFolder");
            var copiedFile1 = Path.Combine(notRealDestination, fileName);

            // Act - Copy the file1 to non-existent directory
            var result = Io.Copy(file1, notRealDestination);

            // Assert
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

            // Create dummy files in the BasePath
            var files = TestHelpers.Disk.CreateFiles(25, BasePath).ToArray();

            // Act - Copy the files
            var result = Io.Copy(_destination, files);

            // Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(Status.Ok, result.Status);
            Assert.IsTrue(Directory.GetFiles(_destination).Length == 25);
        }

        [Test, Order(4)]
        public void Copy_Robocopy_ShouldSucceed()
        {
            // Destination should be empty now
            Assert.IsEmpty(Directory.GetFiles(_destination));

            // Create dummy files in the BasePath
            var binFolder = Path.Combine(BasePath, "bin");
            TestHelpers.Disk.CreateFolder(binFolder);
            (string Path,  string FileName,  bool Copied)[] files = {
                (BasePath, "file.exe", true),
                (BasePath, "file.dll", true),
                (BasePath, "file.exe.config", true),
                (BasePath, "file.pak", true),
                (BasePath, "file.dat", true),
                (binFolder, "file.bin", true),
                (binFolder, "CefSharp.dll", true),
                (BasePath, "file.cache", false),
                (BasePath, "file.txt", false),
                (BasePath, "data.txt", false),
                (binFolder, "file.vshost.exe", false),
                (binFolder, "readme.md", false),
                (binFolder, "file.pdf", false),
                (binFolder, "file.xml", false),
            };

            files.ToList().ForEach(x => TestHelpers.Disk.CreateFile(x.Path, x.FileName));

            // Act - Copy the files
            var result = Io.Copy(BasePath, _destination, "*.dll *.exe *.exe.config CefSharp.* *.pak *.dat *.bin", "/S /XF Destination *.vshost* *.xml *.pdb");

            // Assert
            Assert.IsEmpty(result.Message);
            Assert.AreEqual(Status.Ok, result.Status);
            foreach (var file in files)
            {
                var destinationFile = file.Path == BasePath
                    ? Path.Combine(_destination, file.FileName)
                    : Path.Combine(_destination, "bin", file.FileName);
                if (file.Copied)
                {
                    Assert.IsTrue(File.Exists(destinationFile), $"Expected `{destinationFile}` to exists but it did not.");
                }
                else
                {
                    Assert.IsFalse(File.Exists(destinationFile), $"Expected `{destinationFile}` not to exists but it did.");
                }
            }
        }

        [Test, Order(5)]
        public void Copy_RobocopyWithInvalidParameters_ShouldFail()
        {
            // Act - Copy the files
            var result = Io.Copy(BasePath, _destination, "*.*", "/I_am_an_invalid_parameter");

            // Assert
            Assert.AreEqual(Status.Error, result.Status);
            StringAssert.StartsWith("Robocopy ERROR : Invalid Parameter", result.Message);
            Assert.IsEmpty(Directory.GetFiles(_destination));
        }

        [Test, Order(6)]
        public void Copy_RobocopyWithSourceDoesNotExist_ShouldFail()
        {
            // Arrange
            var randomFolderName = Path.GetRandomFileName();

            // Act - Copy the files
            var result = Io.Copy(Path.Combine(BasePath, randomFolderName), _destination, "*.*", "");

            // Assert
            Assert.AreEqual(Status.Error, result.Status);
            StringAssert.StartsWith("Robocopy ERROR 2: Accessing Source Directory", result.Message);
            Assert.IsEmpty(Directory.GetFiles(_destination));
        }

        [Test, Order(7)]
        public void Copy_FileDoesNotExist_ShouldFail()
        {
            // Destination should be empty now
            Assert.IsEmpty(Directory.GetFiles(_destination));
            
            // Act - Copy the non-existent file
            var file1 = Path.Combine(BasePath, "i_dont_exist.txt");
            var result = Io.Copy(file1, _destination);

            // Assert
            StringAssert.StartsWith("Could not find file", result.Message);
            Assert.IsFalse(File.Exists(file1));
        }
    }
}
