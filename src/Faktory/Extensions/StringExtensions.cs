using System.IO;
using System.Linq;

namespace Faktory.Core.Extensions;

public static class StringExtensions
{
    public static bool HasDirectoryEnding(this string source) => source.EndsWithAny(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    public static bool EndsWithAny(this string source, params char[] values) => values.Any(c => source.EndsWith(c.ToString()));
}