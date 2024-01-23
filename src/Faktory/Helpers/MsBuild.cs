using System;
using System.IO;

namespace Faktory.Core.Helpers;

/// <summary>
/// Helpers for working with MSBuild.
/// </summary>
public static class MsBuild
{
    public static string MsBuildPath => Config.Get("MSBuildPath");
    public static ProcessStepResult Clean(string solutionPath, string configuration = "Debug", string platform = "x64")
    {
        return Run(solutionPath, configuration, platform, "Clean");
    }

    public static ProcessStepResult Run(string solutionPath, string configuration = "Debug", string platform = "x64", string target = "Build", string args = null)
    {
        if (MsBuildExists() == false)
        {
            return new ProcessStepResult(Status.Error, $"Config option 'MSBuildPath' not set. Please override Configure().");
        }

        if (File.Exists(solutionPath) == false)
        {
            return new ProcessStepResult(Status.Error, $"Could not find '{solutionPath}'");
        }

        return ExecuteMsBuild(solutionPath, configuration, platform, target, args);
    }

    static ProcessStepResult ExecuteMsBuild(string solutionPath, string configuration, string platform, string target, string args)
    {
        try
        {
            return Process.Run(MsBuildPath, GetArguments(solutionPath, configuration, platform, target, args));
        }
        catch (Exception e)
        {
            return new ProcessStepResult(Status.Error, e.Message);
        }
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

        var result = Process.Run(msBuildPath, "-version");
        return result.ExitCode == 0;
    }

}