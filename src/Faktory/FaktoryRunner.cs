using System;
using Faktory.Core.Logging;

namespace Faktory.Core;

public static class FaktoryRunner
{
    /// <summary>
    /// Used to write the log to the console.
    /// Useful to overwrite for unit testing.
    /// </summary>
    public static ILogWriter LogWriter { get; set; }

    public static bool BootUp(string args, Action<string> updateStatus)
    {
        return Boot.Up(args, updateStatus);
    }

    public static bool Run(Faktory faktory)
    {
        LogWriter ??= new SpectreLogWriter();
        faktory.RunBuild();
        if (faktory.Executed == false)
        {
            Boot.Logger.Error("Please call Execute() method.");
        }

        faktory.WriteLog(LogWriter);

        return faktory.Executed;
    }
}