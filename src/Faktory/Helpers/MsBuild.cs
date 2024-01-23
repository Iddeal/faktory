using System;
using System.IO;

namespace Faktory.Core.Helpers;

/// <summary>
/// Helpers for working with MSBuild.
/// </summary>
public static class MsBuild
{
    public static string MsBuildPath => Config.Get("MSBuildPath");
    public static void Clean(string solutionPath, string configuration = "Debug", string platform = "x64")
    {
        Run(solutionPath, configuration, platform, "Clean");
    }

    public static void Run(string solutionPath, string configuration = "Debug", string platform = "x64", string target = "Build", string args = null)
    {
        ExecuteMsBuild(solutionPath, configuration, platform, target, args);
    }

    static void ExecuteMsBuild(string solutionPath, string configuration, string platform, string target, string args)
    {
        if (MsBuildExists() == false)
        {
            throw new Exception($"Config option 'MSBuildPath' not set. Please override Configure().");
        }

        if (File.Exists(solutionPath) == false)
        {
            throw new Exception($"Could not find '{solutionPath}'");
        }

        Process.Run(MsBuildPath, GetArguments(solutionPath, configuration, platform, target, args));
    }

    static string GetArguments(string solutionPath, string configuration, string platform, string target, string args)
    {
        var finalArgs = $"{solutionPath} /p:Configuration={configuration} /p:Platform=\"{platform}\" /t:{target}";
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