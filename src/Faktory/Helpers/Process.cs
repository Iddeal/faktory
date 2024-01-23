﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <returns></returns>
    public static void Run(string command, string arguments = "", string workingDirectory = null)
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

            process.Start();
            process.WaitForExit();

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

            if (process.ExitCode != 0) throw new Exception($"Process exited with code {process.ExitCode}");
        }
        catch (Exception e)
        {
            throw new Exception($"Error running `{command}`: {e.Message}");
        }
    }
}