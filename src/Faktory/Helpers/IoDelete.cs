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
        public static BuildStepResult DeleteFile(string file)
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

                return new BuildStepResult(Status.Ok, string.Empty);
            }
            catch (Exception e)
            {
                Boot.Logger.Error($"Error deleting `{file}`: {e.Message}");
                return new BuildStepResult(Status.Error, e.Message);
            }
        }

        /// <summary>
        /// Deletes each file in <paramref name="files"/>.
        /// </summary>
        /// <param name="files">List of files to delete.</param>
        /// <returns></returns>
        public static BuildStepResult DeleteFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var result = DeleteFile(file);
                if (result.Status == Status.Error)
                {
                    return result;
                }
            }
            return new BuildStepResult(Status.Ok, string.Empty);
        }

        /// <summary>
        /// Deletes a directory.
        /// </summary>
        /// <param name="directory">The directory to delete.</param>
        /// <returns></returns>
        public static BuildStepResult DeleteDirectory(string directory)
        {
            try
            {
                Boot.Logger.Info($"Deleting directory: `{directory}`");
                Directory.Delete(directory, true);
                return new BuildStepResult(Status.Ok, string.Empty);
            }
            catch (Exception e)
            {
                Boot.Logger.Error($"Error deleting `{directory}`: {e.Message}");
                return new BuildStepResult(Status.Error, e.Message);
            }
        }
    }