using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Faktory.Tests
{
    public static class TestHelpers
    {
        public static class Disk
        {
            public static void LockFile(string filePath, int secondsToWait)
            {
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    // Wait for the specified seconds or until the lock is released
                    fs.Lock(0, fs.Length);
                    Thread.Sleep(secondsToWait * 1000);
                }
            }

            public static string CreateFile(string path, string name)
            {
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                var fileName = Path.Combine(path, name);
                var fs = File.Create(fileName);
                fs.Close();
                return fileName;
            }

            public static IEnumerable<string> CreateFiles(int howMany, string path)
            {
                for (var i = 0; i < howMany; i++)
                {
                    yield return CreateFile(path, $"test-file{i}.txt");
                }
            }

            public static void CreateFolder(string path)
            {
                Directory.CreateDirectory(path);
            }

            public static IEnumerable<string> CreateFolderWithFiles(string path, int howMany)
            {
                Directory.CreateDirectory(path);
                return CreateFiles(howMany, path);
            }

            public static IEnumerable<string> CreateFoldersWithFiles(string root, int howMany)
            {
                var files = new List<string>();
                for (var i = 0; i < howMany; i++)
                {
                     files.AddRange(CreateFolderWithFiles(Path.Combine(root, $"folder{i}"), howMany));
                }

                return files;
            }
        }
    }
}