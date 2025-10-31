using System;
using System.IO;

namespace Faktory.Core.Helpers;

/// <summary>
/// Helpers for working with MSBuild.
/// </summary>
public static class MsBuild
{
    public static string MsBuildPath => Config.Get("MSBuildPath");
    public static void Clean(string solutionPath, string configuration = "Debug", string platform = "Any CPU")
    {
        Run(solutionPath, configuration, platform, "Clean");
    }

    public static void Run(string solutionPath, string configuration = "Debug", string platform = "Any CPU", string target = "Build", bool optimize = false, string outputDir = null, string args = null)
    {
        ExecuteMsBuild(solutionPath, configuration, platform, target, optimize, outputDir, args);
    }

    static void ExecuteMsBuild(string solutionPath, string configuration, string platform, string target, bool optimize, string outputDir, string args)
    {
        if (MsBuildExists() == false)
        {
            throw new Exception($"Config option 'MSBuildPath' not set. Please override Configure().");
        }

        if (File.Exists(solutionPath) == false)
        {
            throw new Exception($"Could not find '{solutionPath}'");
        }

        Process.Run(MsBuildPath, GetArguments(solutionPath, configuration, platform, target, optimize, outputDir, args));
    }

    static string GetArguments(string solutionPath, string configuration, string platform, string target, bool optimize, string outputDir, string args)
    {
        var finalArgs = $"{solutionPath} /p:Configuration={configuration} /p:Platform=\"{platform}\" /t:{target} /p:Optimize={optimize}";
        if (string.IsNullOrEmpty(outputDir) == false)
        {
            finalArgs += $@" /p:OutputPath={outputDir}";
        }

        return string.IsNullOrEmpty(args) ? finalArgs : $"{finalArgs} {args}";
    }

    static bool MsBuildExists()
    {
        var msBuildPath = MsBuildPath;
        if (string.IsNullOrEmpty(msBuildPath)) return false;
        if (File.Exists(msBuildPath) == false) return false;

        try
        {
            Process.Run(msBuildPath, "-version");
            return true;
        }
        catch 
        {
            return false;
        }
        
    }
}