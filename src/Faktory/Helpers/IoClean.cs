using System;
using System.IO;
using Faktory.InternalUtilities;

namespace Faktory.Helpers
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
        public static BuildStepResult CleanDirectory(string path)
        {
            try
            {
                // Recursively delete all the files and folders in the path.
                foreach (var file in Directory.GetFiles(path))
                {
                    var (inUse, processName) = FileUsage.GetFileUsage(file);
                    if (inUse)
                    {
                        throw new Exception($"Can't delete `{file}`. It's locked by {processName}.");
                    }
                    Boot.Logger.Info($"Deleting file: `{file}`");
                    File.Delete(file);
                }

                foreach (var directory in Directory.GetDirectories(path))
                {
                    Boot.Logger.Info($"Deleting folder: `{directory}`");
                    CleanDirectory(directory);
                    Directory.Delete(directory);
                }

                return new BuildStepResult(Status.Ok, string.Empty);
            }
            catch (Exception e)
            {
                Boot.Logger.Error($"Error cleaning `{path}`: {e.Message}");
                return new BuildStepResult(Status.Error, e.Message);
            }
        }
    }
}