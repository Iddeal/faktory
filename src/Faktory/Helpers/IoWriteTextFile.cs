using System.IO;

namespace Faktory.Core.Helpers;

/// <summary>
/// Set of helpers for working with Disk operations.
/// </summary>
public static partial class Io
{
    /// <summary>
    /// Writes the given contents to the specific file. If it already exists, it is overwritten.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="contents"></param>
    public static void WriteTextFile(string path, string contents)
    {
        File.WriteAllText(path, contents);
    }
}