using System.Collections.Generic;

namespace Faktory.Core;

public static class Config
{
    static Dictionary<string, string> _configurations = new();
    public static string Get(string key) => _configurations[key];
    public static string Set(string key, string value) => _configurations[key] = value;

    public static void Reset()
    {
        _configurations = new();
    }
}