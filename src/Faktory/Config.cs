using System.Collections.Generic;

namespace Faktory.Core;

public class Config
{
    readonly Dictionary<string, string> _configurations = new();

    public string this[string key]
    {
        get => _configurations[key];
        set => _configurations[key] = value;
    }
}