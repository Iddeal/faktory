using System;
using System.Collections.Generic;
using System.IO;
using Faktory.Core.InternalUtilities;

namespace Faktory.Core.Helpers;

 public static partial class Io
    {
        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="file">Full path to file being deleted.</param>
        /// <returns></returns>
        public static void DeleteFile(string file)
        {
            try
            {
                var (inUse, processName) = FileUsage.GetFileUsage(file);
                if (inUse)
                {
                    throw new Exception($"Can't delete `{file}`. It's locked by {processName}.");
                }
                Boot.Logger.Info($"Deleting file: `{file}`");
                File.Delete(file);
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting `{file}`: {e.Message}");
            }
        }

        /// <summary>
        /// Deletes each file in <paramref name="files"/>.
        /// </summary>
        /// <param name="files">List of files to delete.</param>
        /// <returns></returns>
        public static void DeleteFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                DeleteFile(file);
            }
        }

        /// <summary>
        /// Deletes a directory.
        /// </summary>
        /// <param name="directory">The directory to delete.</param>
        /// <returns></returns>
        public static void DeleteDirectory(string directory)
        {
            try
            {
                Boot.Logger.Info($"Deleting directory: `{directory}`");
                Directory.Delete(directory, true);
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting `{directory}`: {e.Message}");
            }
        }
    }