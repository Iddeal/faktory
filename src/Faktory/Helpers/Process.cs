using System;
using System.Diagnostics;

namespace Faktory.Helpers;

public static class Process
{
    /// <summary>
    /// Runs an external process.
    /// </summary>
    /// <param name="command">Process to run.</param>
    /// <param name="arguments">Optional arguments</param>
    /// <param name="workingDirectory">Defaults to script directory.</param>
    /// <returns></returns>
    public static ProcessStepResult Run(string command, string arguments = "", string workingDirectory = null)
    {
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


            process.Start();
            process.WaitForExit();

            //Capture the output to the log
            while (process.StandardOutput.EndOfStream == false)
            {
                var dataLine = process.StandardOutput.ReadLine();
                if (dataLine == null) continue;

                Boot.Logger.Info(dataLine);
            }

            //Capture the errors to the log
            while (process.StandardError.EndOfStream == false)
            {
                var dataLine = process.StandardError.ReadLine();
                if (dataLine == null) continue;

                Boot.Logger.Error(dataLine);
            }

            return process.ExitCode != 0
                ? new ProcessStepResult(Status.Error, string.Empty, process.ExitCode)
                : new ProcessStepResult(Status.Ok, string.Empty, process.ExitCode);
        }
        catch (Exception e)
        {
            Boot.Logger.Error($"Error running `{command}`: {e.Message}");
            return new ProcessStepResult(Status.Error, e.Message);
        }
    }
}