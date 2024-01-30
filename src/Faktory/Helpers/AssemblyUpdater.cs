using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Faktory.Core.Helpers;

public class AssemblyUpdater
{
    public struct AssemblyInfo{
        public string Title {get;set;}
        public string Description { get; set; }
        public string Version { get; set; }
        public string FileVersion { get; set; }

        public static AssemblyInfo Default()
        {
            return new AssemblyInfo();
        }
    }
    
    public static void Update(string filePath, AssemblyInfo attributes){
        if (File.Exists(filePath) == false) { throw new Exception($"AssemblyInfo not found at '{filePath}'"); }

        var originalFile = File.ReadAllText(filePath);
        File.WriteAllText(filePath, UpdateAttributes(originalFile, attributes));
    }
	
    static string UpdateAttributes(string original, AssemblyInfo attributes) => GetProperties(attributes)
        .Aggregate(original, (current, prop) => UpdateAttribute(current, prop.Key, prop.Value));

    static Dictionary<string, string> GetProperties(AssemblyInfo attributes) =>
        attributes.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(attributes, null));

    static string UpdateAttribute(string original, string attribute, string value)
    {
        const RegexOptions options = RegexOptions.Multiline | RegexOptions.None;
        value = $"\"{value}\""; // Need to add quotes around the value
        var pattern = Pattern(attribute);
        var m = Regex.Match(original, pattern, options);
        switch (m.Success)
        {
            case true when m.Value != value:
                Boot.Logger.Verbose($"Attribute[{attribute}] - Setting to {value}");
                return Regex.Replace(original, pattern, value, options);
            case true:
                Boot.Logger.Verbose($"Attribute[{attribute}] - Value already equals {value}");
                return original;
            default:
                throw new Exception($"Attribute[{attribute}] - Could not find attribute");
        }
    }

    static string Pattern(string attribute) => $@"(?<=^\s*\[\s*assembly:\s*Assembly{attribute}(?:Attribute)?\s*\()(.*)(?=\)\s*\]\s*$)";
}

