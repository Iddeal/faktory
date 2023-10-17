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

    /// <summary>
    /// Copies each file in <paramref name="files"/> to <paramref name="destination"/>.
    /// If the file already exists, it is overwritten.
    /// If the <paramref name="destination"/> does not exist, it is created.
    /// </summary>
    /// <param name="files">The file to copy.</param>
    /// <param name="destination">The directory to copy <paramref name="files" /> to.</param>
    /// <returns></returns>
    public static BuildStepResult Copy(string destination, string[] files)
    {
        foreach (var file in files)
        {
            var result = Copy(file, destination);
            if (result.Status == Status.Error)
            {
                return result;
            }
        }
        return new BuildStepResult(Status.Ok, string.Empty);
    }

    /// <summary>
    /// A wrapper around Robocopy.
    /// Refer to its documentation or use the `robocopy /?` command in the command prompt.
    /// </summary>
    /// <param name="source">Specifies the path to the source directory.</param>
    /// <param name="destination">Specifies the path to the destination directory.</param>
    /// <param name="files">Specifies the file or files to be copied. Wildcard characters (* or ?) are supported. If you don't specify this parameter, *.* is used as the default value.</param>
    /// <param name="options">Specifies the options to use with the robocopy command, including copy, file, retry, logging, and job options.</param>
    /// <returns></returns>
    public static BuildStepResult Copy(string source, string destination, string files, string options)
    {
        try
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "robocopy.exe";
            process.StartInfo.Arguments = $"{source} {destination} {files} {options}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            var errorHasOccurred = "";
            while (process.StandardOutput.EndOfStream == false)
            {
                var dataLine = process.StandardOutput.ReadLine();
                if (dataLine == null) continue;

                if (dataLine.ToLowerInvariant().Contains("error"))
                {
                    errorHasOccurred = TryParseRobocopyError(dataLine);
                }
                Boot.Logger.Info(dataLine);
            }

            process.WaitForExit();

            if (string.IsNullOrEmpty(errorHasOccurred) && process.ExitCode < 8) return new BuildStepResult(Status.Ok, string.Empty);

            // An error occurred
            var writer = new TestLogWriter();
            Boot.Logger.Write(writer);

            var error = string.IsNullOrEmpty(errorHasOccurred)
                ? $"Robocopy exited with code {process.ExitCode}"
                : $"Robocopy {errorHasOccurred}";
            return new BuildStepResult(Status.Error, error);
        }
        catch (Exception e)
        {
            Boot.Logger.Error($"Error calling robocopy: {e.Message}");
            return new BuildStepResult(Status.Error, e.Message);
        }
    }

    static string TryParseRobocopyError(string dataLine)
    {
        try
        {
            const string pattern = @"^\d{4}\/\d{2}\/\d{2}\s\d{2}:\d{2}:\d{2}\s+ERROR\s(\d+)\s\(0x[0-9A-Fa-f]+\)\s(.+)";
            var match = Regex.Match(dataLine, pattern);

            if (match.Success == false) return dataLine; // We cannot parse the error so use the entire line

            var errorNumber = match.Groups[1].Value;
            var errorString = match.Groups[2].Value;
            return $"ERROR {errorNumber}: {errorString}";
        }
        catch
        {
            // If we cannot parse the error just return the entire line
            return dataLine;
        }
    }
}