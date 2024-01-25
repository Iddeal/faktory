using System;
using System.Collections.Generic;

namespace Faktory.Core.Logging
{
    internal class Logger
    {
        readonly List<LogLine> _logLines = new();
        public int IndentLevel { get; set; }
        public ILogWriter Writer { get; set; }

        public void Error(Exception exception, bool lineFeed = true) => Error(exception.Message, exception, lineFeed);
        public void Error(string message, Exception exception = null, bool lineFeed = true) => InternalWrite(message, lineFeed, LogType.Error, LogColor.Red, exception);
        public void Warning(string message, bool lineFeed = true) => InternalWrite(message, lineFeed, LogType.Warning, LogColor.Yellow);
        public void Verbose(string message, bool lineFeed = true)
        {
            if (Boot.Options.VerboseMode) InternalWrite(message, lineFeed, LogType.Verbose, LogColor.Green);
        }

        public void Info(string message, LogColor color = LogColor.White, bool lineFeed = true) => InternalWrite(message, lineFeed, LogType.Info, color);

        void InternalWrite(string message, bool lineFeed, LogType logType, LogColor color, Exception e = null)
        {
            var indent = new string(' ', IndentLevel * 4);

            var logLine = new LogLine
            {
                Text = indent + message,
                Timestamp = DateTime.Now,
                IndentLevel = IndentLevel,
                LogType = logType,
                Color = color,
                LineFeed = lineFeed,
                Exception = e
            };
            _logLines.Add(logLine);
            Writer?.Write(logLine);
        }
    }
}