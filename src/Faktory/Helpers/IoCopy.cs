using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Faktory.Core.Extensions;
using Faktory.Core.Logging;

namespace Faktory.Core.Helpers;

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
    public static void Copy(string file, string destination)
    {
        try
        {
            if (Directory.Exists(destination) == false) Directory.CreateDirectory(destination);
            var fileName = Path.GetFileName(file);
            var destinationFile = Path.Combine(destination, fileName);
            Boot.Logger.Info($"Copying `{fileName}` to `{destination}`");
            File.Copy(file, destinationFile, true);
        }
        catch (Exception e)
        {
            throw new Exception($"Error copying `{file}`: {e.Message}");
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
    public static void Copy(string destination, string[] files)
    {
        foreach (var file in files)
        {
            Copy(file, destination);
        }
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
    public static void Copy(string source, string destination, string files, string options = "")
    {
        try
        {
            var arguments = $"{NormalizePath(source)} {NormalizePath(destination)} {files} {options}";

            Boot.Logger.Info($"Running robocopy.exe {arguments}");

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "robocopy.exe";
            process.StartInfo.Arguments = arguments;
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

            if (string.IsNullOrEmpty(errorHasOccurred) && process.ExitCode < 8) return;

            // An error occurred
            var error = string.IsNullOrEmpty(errorHasOccurred)
                ? $"Robocopy exited with code {process.ExitCode}"
                : $"Robocopy {errorHasOccurred}";
            throw new Exception(error);
        }
        catch (Exception e)
        {
            throw new Exception($"Error calling robocopy: {e.Message}");
        }
    }

    /// <summary>
    /// Robocopy requires paths to be quoted and directories to end with a ". ". (E.g., C:\. ) 
    /// </summary>
    static string NormalizePath(string path)
    {
        return path.HasDirectoryEnding() ? $"\"{path}.\" " : $"\"{path}\" ";
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