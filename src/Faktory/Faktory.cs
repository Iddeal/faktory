using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Faktory.Core.Exceptions;
using Faktory.Core.Logging;
using Faktory.Core.ProgressReporter;

namespace Faktory.Core
{
    public class Faktory
    {
        bool _missingRequiredOptions;
        Action<string> _updateStatus;
        List<Action> BuildActions { get; } = new();

        public List<ActionResult> ActionResults { get; } = new();
        public List<string> RequiredOptions { get; } = new();
        /// <summary>
        /// List of options provided as arguments to the CLI.
        /// </summary>
        public Options Options => Boot.Options;
        public IProgressReporter Reporter => FaktoryRunner.ProgressReporter;

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
        /// Causes the build to stop with a failure.
        /// </summary>
        /// <param name="reason"></param>
        public void Fail(string reason)
        {
            throw new BuildFailureException(reason);
        }

        /// <summary>
        /// Run all the tasks defined in your Faktory
        /// </summary>
        public void Execute()
        {
            // Load the custom Config
            try
            {
                Configure();
            }
            catch (Exception e)
            {
                Boot.Logger.Error(e);
                return;
            }

            Executed = true;
            if (_missingRequiredOptions) return;

            if (BuildActions.Any() == false) Boot.Logger.Error("No Tasks found.");

            foreach (var x in BuildActions)
            {
                var methodName = x.Method.Name;
                _updateStatus($"Running {methodName}()");
                var result = new ActionResult { Name = methodName };
                try
                {
                    Boot.Logger.Info($"{methodName}() -> ", LogColor.Green);
                    Boot.Logger.IndentLevel = 1;
                    Reporter.ReportStartProgress(methodName);
                    result.Duration = ExecuteAndTimeAction(x);
                    Reporter.ReportEndProgress(methodName);
                }
                catch (Exception e)
                {
                    result.LastException = e;
                    Boot.Logger.Error(e);
                    return;
                }
                finally
                {
                    Boot.Logger.IndentLevel = 0;
                    if (result.LastException != null) Reporter.ReportFailure($"{methodName} failed with - {result.LastException.Message}");
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