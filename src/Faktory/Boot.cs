using System;
using System.Linq;
using Faktory.Core.Logging;

namespace Faktory.Core
{
    internal static class Boot
    {
        public static Options Options { get; private set; }
        public static Logger Logger { get; private set; } = new();
        public static string SourcePath { get; } = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Load all information requires for Faktory to run.
        /// Each step accepts a StatusContext and the arguments passed to the program
        /// </summary>
        /// <param name="args"></param>
        public static bool Up(string args, Action<string> updateStatus)
        {
            Logger = new Logger();
            Logger.Writer = new SpectreLogWriter();
            return With(updateStatus, args, CaptureOptions, DisplayOptions);
        }

        static bool With(Action<string> updateStatus, string args, params Func<Action<string>, string, bool>[] actions)
        {
            return actions.All(action => action(updateStatus, args));
        }

        static bool CaptureOptions(Action<string> updateStatus, string args)
        {
            updateStatus("Parsing options...");
            Options = new Options(args);
            if (Options.Valid == false)
            {
                foreach (var option in Options.InvalidOptions)
                {
                    Logger.Error($"Argument '{option.Key}' has invalid value.");
                }
            }
            return Options.Valid;
        }

        static bool DisplayOptions(Action<string> updateStatus, string args)
        {
            updateStatus("Logging options...");
            Logger.Info("Options: [", lineFeed: false);
            foreach (var option in Options)
            {
                Logger.Info("{'", lineFeed: false);
                Logger.Info(option.Key, lineFeed: false, color: LogColor.Yellow);
                Logger.Info("'->'", lineFeed: false);
                Logger.Info(option.Value, lineFeed: false, color: LogColor.Blue);
                Logger.Info("'}", lineFeed: false);
            }

            Logger.Info("]");
            return true;
        }
    }
}