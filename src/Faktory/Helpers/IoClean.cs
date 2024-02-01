using System;
using System.IO;
using System.Text;
using Faktory.Core.InternalUtilities;

namespace Faktory.Core.Helpers
{
    /// <summary>
    /// Set of helpers for working with Disk operations.
    /// </summary>
    public static partial class Io
    {
        /// <summary>
        /// Deletes all the files and folders in a directory. Directory is created if it does not exist.
        /// </summary>
        /// <param name="path">Directory to be cleaned.</param>
        /// <returns></returns>
        public static void CleanDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                    return;
                }

                // Recursively delete all the files and folders in the path.
                foreach (var file in Directory.GetFiles(path))
                {
                    try
                    {
                        Boot.Logger.Info($"Deleting file: `{file}`");
                        File.Delete(file);
                    }
                    catch 
                    {
                        var (inUse, processName) = FileUsage.GetFileUsage(file);
                        if (inUse)
                        {
                            throw new Exception($"Can't delete `{file}`. It's locked by {processName}.");
                        }

                        throw;
                    }
                }

                foreach (var directory in Directory.GetDirectories(path))
                {
                    Boot.Logger.Info($"Deleting folder: `{directory}`");
                    CleanDirectory(directory);
                    Directory.Delete(directory);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error cleaning `{path}`: {e.Message}");
            }
        }
    }
}