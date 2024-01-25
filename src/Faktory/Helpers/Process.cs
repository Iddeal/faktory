using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Faktory.Core.Exceptions;

namespace Faktory.Core.Helpers;

public static class Process
{
    /// <summary>
    /// Runs an external process.
    /// </summary>
    /// <param name="command">Process to run.</param>
    /// <param name="arguments">Optional arguments</param>
    /// <param name="workingDirectory">Defaults to script directory.</param>
    /// <param name="validExitCodes">List of exit codes that are considered Success.</param>
    /// <returns></returns>
    public static void Run(string command, string arguments = "", string workingDirectory = null, params int[] validExitCodes)
    {
        var standardOut = new List<string>();
        var standardError = new List<string>();
        try
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workingDirectory ?? Boot.SourcePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Boot.Logger.Info($"Running '{command} {arguments}'...");
            process.Start();

            //Capture the output to the log
            while (process.StandardOutput.EndOfStream == false)
            {
                var dataLine = process.StandardOutput.ReadLine();
                if (dataLine == null) continue;

                Boot.Logger.Info(dataLine);
                standardOut.Add(dataLine);
            }

            //Capture the errors to the log
            while (process.StandardError.EndOfStream == false)
            {
                var dataLine = process.StandardError.ReadLine();
                if (dataLine == null) continue;

                Boot.Logger.Error(dataLine);
                standardError.Add(dataLine);
            }
            
            process.WaitForExit();

            if (process.ExitCode != 0 && !validExitCodes.Contains(process.ExitCode)) throw new Exception($"Process exited with code {process.ExitCode}");
        }
        catch (Exception e)
        {
            throw new Exception($"Error running `{command}`: {e.Message}");
        }
    }
}