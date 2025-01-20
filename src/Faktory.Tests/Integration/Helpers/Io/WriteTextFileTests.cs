using System.IO;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.Io
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class WriteTextFileTests
    {
        const string BasePath = @".\WriteTextFile Folder";

        [SetUp]
        public void ResetBasePath()
        {
            if (Directory.Exists(BasePath))
            {
                Directory.Delete(BasePath, true);
            }

            TestHelpers.Disk.CreateFolder(BasePath);
        }

        [Test, Order(1)]
        [NonParallelizable]
        public void WriteTextFile_ShouldSucceed()
        {
            const string expectedContent = "This is some output.";
            Assert.IsEmpty(Directory.GetFiles(BasePath), "BaseDirectory is not empty!");
            var file = Path.Combine(BasePath, "write-text-test.txt");

            // Act - Write
            Core.Helpers.Io.WriteTextFile(file, expectedContent);

            // Assert
            Assert.IsTrue(File.Exists(file), $"`{file}` not found");
            Assert.AreEqual(expectedContent, File.ReadAllText(file));
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void WriteTextFile_FileAlreadyExists_ShouldOverwrite()
        {
            const string expectedContent = "This is some output.";
            Assert.IsEmpty(Directory.GetFiles(BasePath), "BaseDirectory is not empty!");
            var file = Path.Combine(BasePath, "write-text-test.txt");
            Core.Helpers.Io.WriteTextFile(file, "Some Other contents");

            // Act - Write
            Core.Helpers.Io.WriteTextFile(file, expectedContent);

            // Assert
            Assert.AreEqual(expectedContent, File.ReadAllText(file));
        }
    }
}