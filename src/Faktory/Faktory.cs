using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Faktory.Core.Logging;

namespace Faktory.Core
{
    public class Faktory
    {
        bool _missingRequiredOptions;
        Action<string> _updateStatus;
        List<Action> BuildActions { get; } = new();

        public List<ActionResult> ActionResults { get; } = new();
        public List<string> RequiredOptions { get; } = new();
        public Options Options => Boot.Options;

        /// <summary>
        /// Returns the path where the Faktory is running from.
        /// </summary>
        public static string SourcePath => Boot.SourcePath;
        public bool Executed { get; private set; }

        protected virtual void Configure() {
            Boot.Logger.Error("Loading with default config.");
        }

        protected internal virtual void RunBuild() {
            Boot.Logger.Error("Please override the RunBuild() method.");
        }

        /// <summary>
        /// Output a line to the final log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="lineFeed"></param>
        public void Log(string message, LogColor color = LogColor.White, bool lineFeed = true) => Boot.Logger.Info(message, color, lineFeed);

        /// <summary>
        /// Not for external use.
        /// </summary>
        /// <param name="logWriter"></param>
        public void WriteLog(ILogWriter logWriter) => Boot.Logger.Write(logWriter);

        /// <summary>
        /// Run all the tasks defined in your Faktory
        /// </summary>
        public void Execute()
        {
            // Load the custom Config
            Configure();

            Executed = true;
            if (_missingRequiredOptions) return;

            if (BuildActions.Any() == false) Boot.Logger.Error("No Tasks found.");

            foreach (var x in BuildActions)
            {
                _updateStatus($"Running {x.Method.Name}()");
                var result = new ActionResult { Name = x.Method.Name };
                try
                {
                    Boot.Logger.Info($"{x.Method.Name}() -> ", LogColor.Green);
                    Boot.Logger.IndentLevel = 1;
                    result.Duration = ExecuteAndTimeAction(x);;
                    result.Success = true;
                }
                catch (Exception e)
                {
                    result.Success = false;
                    Boot.Logger.Error(e);
                    return;
                }
                finally
                {
                    Boot.Logger.IndentLevel = 0;
                    ActionResults.Add(result);
                }
            }
        }

        static TimeSpan ExecuteAndTimeAction(Action x)
        {
            var sw = new Stopwatch();
            sw.Start();
             x.Invoke();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// Alias for <see cref="Run" />.
        /// </summary>
        /// <param name="action"></param>
        public Faktory Then(Action action) => Run(action);

        /// <summary>
        /// Adds a task to be run on <see cref="Execute"/>.
        /// </summary>
        /// <param name="action"></param>
        public Faktory Run(Action action)
        {
            BuildActions.Add(action);
            return this;
        }

        /// <summary>
        /// Specify the required parameters for your Faktory.
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected Faktory Requires(params string[] args)
        {
            foreach (var arg in args)
            {
                RequiredOptions.Add(arg.ToLower());
            }
            _missingRequiredOptions = Boot.Options.HasAll(RequiredOptions) == false;

            return this;
        }

        public void SetStatusUpdater(Action<string> updateStatus)
        {
            _updateStatus = updateStatus;
        }
    }
}