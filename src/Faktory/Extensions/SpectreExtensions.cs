using Faktory.Logging;

namespace Faktory.Extensions;

public static class SpectreExtensions
{
    public static string Colorify(this string source, LogColor color) => $"[{color.Text()}]{source}[/]";
}

public static class ColorExtensions
{
    public static string Text(this LogColor source) => source.ToString().ToLower();
}