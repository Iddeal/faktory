using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Faktory.Core.InternalUtilities;
using NUnit.Framework;

namespace Faktory.Tests.Integration.InternalUtilities
{
    [TestFixture]
    public class FileUsageTests
    {
        static string _filePath;
        const string BasePath = "./CleanTestFolder";

        [OneTimeSetUp]
        public void Init()
        {
            TestHelpers.Disk.CreateFile(BasePath, "LockingTest.txt");
            _filePath = Path.Combine(BasePath, "LockingTest.txt");
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            Directory.Delete(BasePath, true);
        }

        [Test]
        [NonParallelizable]
        public void FileUsage_ReturnsNameOfProcessLockingFile()
        {
            // Create file and lock it
            Task.Run(() => TestHelpers.Disk.LockFile(_filePath, 2));

            var (inUse, processName) = FileUsage.GetFileUsage(_filePath);
            Thread.Sleep(2000); // Wait for the lock release before running other tests

            Assert.IsTrue(inUse);
            Assert.IsNotNull(processName);
            Assert.IsNotEmpty(processName);
        }
    }
}
