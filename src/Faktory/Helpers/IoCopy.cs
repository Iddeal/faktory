using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Faktory.Logging;

namespace Faktory.Helpers;

/// <summary>
/// Set of helpers for working with Disk operations.
/// </summary>
public static partial class Io
{
    /// <summary>
    /// Copies <paramref name="file"/> to <paramref name="destination"/>.
    /// If the file already exists, it is overwritten.
    /// If the <paramref name="destination"/> does not exist, it is created.
    /// </summary>
    /// <param name="file">The file to copy.</param>
    /// <param name="destination">The directory to copy <paramref name="file" /> to.</param>
    /// <returns></returns>
    public static BuildStepResult Copy(string file, string destination)
    {
        try
        {
            if (Directory.Exists(destination) == false) Directory.CreateDirectory(destination);
            var fileName = Path.GetFileName(file);
            var destinationFile = Path.Combine(destination, fileName);
            Boot.Logger.Info($"Copying `{fileName}` to `{destination}`");
            File.Copy(file, destinationFile, true);
            return new BuildStepResult(Status.Ok, string.Empty);
        }
        catch (Exception e)
        {
            Boot.Logger.Error($"Error copying `{file}`: {e.Message}");
            return new BuildStepResult(Status.Error, e.Message);
        }
    }

}