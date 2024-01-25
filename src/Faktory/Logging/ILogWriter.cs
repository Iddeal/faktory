using System;
using System.Collections.Generic;
using Spectre.Console;

namespace Faktory.Core.Logging;

public interface ILogWriter
{
    void Write(LogLine line);
    void Write(List<LogLine> lines);
}

/// <summary>
/// Simple text output with no formatting or colors.
/// </summary>
public class ConsoleLogWriter : ILogWriter
{
    public void Write(List<LogLine> lines)
    {
        foreach (var line in lines)
        {
            Write(line);
        }
    }

    public void Write(LogLine line)
    {
        var linedFeed = line.LineFeed ? Environment.NewLine : "";
        var message = $"{line.Text}{linedFeed}";
        Console.Write(message);
    }
}

/// <summary>
/// Captures the entire log to a List<string>()
/// </summary>
public class TestLogWriter : ILogWriter
{
    public TestLogWriter()
    {
    }

    public TestLogWriter(Action<LogLine> testWrite)
    {
        TestWrite = testWrite;
    }

    public List<string> AllMessages { get; } = new();
    public Action<LogLine> TestWrite { get; set; }

    string _buffer = "";

    public void Write(List<LogLine> lines)
    {
        var parsedLine = "";
        var parsingLines = false;
        foreach (var line in lines)
        {
            var content = $"{line.Text}";
            if (line.LineFeed)
            {
                if (parsingLines)
                {
                    content = parsedLine + content;
                    parsingLines = false;
                }
                AllMessages.Add(content);
            }
            else
            {
                parsingLines = true;
                parsedLine += content;
            }
        }
    }

    public void Write(LogLine line)
    {
        _buffer += line.Text;
        if (line.LineFeed)
        {
            AllMessages.Add(_buffer);
            _buffer = "";
        }

        TestWrite?.Invoke(line);
    }
}

/// <summary>
/// Writes the line to the AnsiConsole using colors and formatting.
/// </summary>
public class SpectreLogWriter : ILogWriter
{
    readonly Dictionary<LogColor, string> _colorMap = new()
    {
        { LogColor.Blue, "aqua" },
        { LogColor.Green, "lime" },
        { LogColor.Purple, "purple" },
        { LogColor.Red, "red" },
        { LogColor.White, "white" },
        { LogColor.Yellow, "yellow" }
    };

    public void Write(List<LogLine> lines)
    {
        var parsedLine = "";
        bool parsingLines = false;
        foreach (var line in lines)
        {
            var content = $"[{_colorMap[line.Color]}]{line.Text.EscapeMarkup()}[/]";
            if (line.LineFeed)
            {
                if (parsingLines)
                {
                    content = parsedLine + content;
                    parsingLines = false;
                }
                AnsiConsole.MarkupLine(content);
            }
            else
            {
                parsingLines = true;
                parsedLine += content;
            }
        }
    }

    public void Write(LogLine line)
    {
        var content = $"[{_colorMap[line.Color]}]{line.Text.EscapeMarkup()}[/]";
        if (line.LineFeed)
        {
            AnsiConsole.MarkupLine(content);
        }
        else
        {
            AnsiConsole.Markup(content);
        }
    }
}