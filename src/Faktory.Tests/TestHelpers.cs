using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Faktory.Tests
{
    public static class TestHelpers
    {
        public static class Disk
        {
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

            public static void CreateFolder(string path)
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}