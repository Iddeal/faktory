using System.Collections.Generic;
using System.IO;

namespace Faktory.Core.Helpers;

/// <summary>
/// Set of helpers for working with Disk operations.
/// </summary>
public static partial class Io
{
    /// <summary>
    /// Recursively search for matching files under the path including subfolders.
    /// </summary>
    /// <param name="path">Directory to search.</param>
    /// <param name="pattern">The search string to match against the names of files in <paramref name="path" />.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    /// <returns></returns>
    public static IEnumerable<string> GetAllFilesMatching(string path, string pattern)
    {
        foreach (var file in Directory.EnumerateFiles(path, pattern))
        {
            yield return file;
        }

        foreach (var d in Directory.EnumerateDirectories(path))
        {
            foreach (var file in GetAllFilesMatching(d, pattern))
            {
                yield return file;
            }
        }
    }
}