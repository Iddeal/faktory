using System;

namespace Faktory.Logging;

public class LogLine
{
    public string Text { get; set; }
    public LogType LogType { get; set; }
    public LogColor Color { get; set; }
    public DateTime Timestamp { get; set; }
    public int IndentLevel { get; set; }
    public bool LineFeed { get; set; }
    public Exception Exception { get; set; }

    public override string ToString()
    {
        var indent = IndentLevel > 0 ? new string(' ', IndentLevel * 4) : string.Empty;
        return $"[{Timestamp:G}] {Text}";
    }
}