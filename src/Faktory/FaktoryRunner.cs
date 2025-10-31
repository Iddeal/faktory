using System;
using Faktory.Core.InternalUtilities;
using Faktory.Core.Logging;
using Faktory.Core.ProgressReporter;

namespace Faktory.Core;

public static class FaktoryRunner
{
    /// <summary>
    /// Used to write the log to the console.
    /// Useful to overwrite for unit testing.
    /// </summary>
    public static ILogWriter LogWriter
    {
        get => Boot.Logger.Writer;
        set => Boot.Logger.Writer = value;
    }
    public static IProgressReporter ProgressReporter { get; set; }

    public static bool BootUp(string args, Action<string> updateStatus)
    {
        LogWriter ??= new SpectreLogWriter();
        ProgressReporter ??= Boot.GetCiRunner() == CiRunners.TeamCity
            ? new TeamCityProgressReporter() 
            : new NullProgressReporter();
        return Boot.Up(args, updateStatus, LogWriter);
    }

    public static bool Run(Faktory faktory)
    {
        faktory.RunBuild();
        if (faktory.Executed == false)
        {
            Boot.Logger.Error("Please call Execute() method.");
        }

        return faktory.Executed && faktory.ShowSummary;
    }

}