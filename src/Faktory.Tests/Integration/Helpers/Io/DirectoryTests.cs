using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.Io
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class DirectoryTests
    {
        const string BasePath = "./GlobbingTests";

        [SetUp]
        public void ResetDestination()
        {
            TestHelpers.Disk.Reset(BasePath);
        }

        [TearDown]
        public void CleanUp()
        {
            TestHelpers.Disk.Reset(BasePath);
        }

        [Test, Order(1)]
        [NonParallelizable]
        public void GetAllFilesMatching_NestedFoldersEachWithJpegs_ReturnsEveryJpegFromAllNestedFolders()
        {
            // Arrange 
            TestHelpers.Disk.CreateFile(BasePath, "I_am_a_base.dll");
            TestHelpers.Disk.CreateFile(BasePath, "I_am_a_base.txt");
            TestHelpers.Disk.CreateFile(BasePath, "I_am_a_base.jpg");
            TestHelpers.Disk.CreateFile(Path.Combine(BasePath, "FolderA"), "I_am_a_FolderA.png");
            TestHelpers.Disk.CreateFile(Path.Combine(BasePath, "FolderA"), "I_am_a_FolderA.jpg");
            TestHelpers.Disk.CreateFile(Path.Combine(BasePath, "FolderA"), "FolderA_123.jpg");
            TestHelpers.Disk.CreateFile(Path.Combine(BasePath, "FolderA"), "FolderA_456.jpg");
            TestHelpers.Disk.CreateFile(Path.Combine(BasePath, "FolderA"), "FolderA_789.jpg");
            TestHelpers.Disk.CreateFile(Path.Combine(BasePath, "FolderB"), "FolderB_B12.jpg");
            TestHelpers.Disk.CreateFile(Path.Combine(BasePath, "FolderC"), "FolderC_C12.jpg");
            TestHelpers.Disk.CreateFile(Path.Combine(BasePath, "FolderD"), "FolderD_D12.jpg");
            TestHelpers.Disk.CreateFile(Path.Combine(Path.Combine(BasePath, "FolderD"), "ThreeDeepFolderA"), "ThreeDeepFolderA_ABC.jpg");

            // Act - Clean the path
            var files = Core.Helpers.Io.GetAllFilesMatching(BasePath, "*.jpg").ToList();

            // Assert
            CollectionAssert.Contains(files, @"./GlobbingTests\I_am_a_base.jpg");
            CollectionAssert.Contains(files, @"./GlobbingTests\FolderA\FolderA_123.jpg");
            CollectionAssert.Contains(files, @"./GlobbingTests\FolderA\FolderA_456.jpg");
            CollectionAssert.Contains(files, @"./GlobbingTests\FolderA\FolderA_789.jpg");
            CollectionAssert.Contains(files, @"./GlobbingTests\FolderA\I_am_a_FolderA.jpg");
            CollectionAssert.Contains(files, @"./GlobbingTests\FolderB\FolderB_B12.jpg");
            CollectionAssert.Contains(files, @"./GlobbingTests\FolderC\FolderC_C12.jpg");
            CollectionAssert.Contains(files, @"./GlobbingTests\FolderD\FolderD_D12.jpg");
            CollectionAssert.Contains(files, @"./GlobbingTests\FolderD\ThreeDeepFolderA\ThreeDeepFolderA_ABC.jpg");
            Assert.That(files.Count(), Is.EqualTo(9));
        }
    }
}