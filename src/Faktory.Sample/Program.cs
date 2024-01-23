using System;
using System.Linq;
using System.Threading;
using Faktory.Core;
using Faktory.Core.Helpers;
using Faktory.Core.Logging;
using static Faktory.Core.Helpers.Io;

namespace Faktory.Sample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            FaktoryProgram.Run(args);
        }
    }

    public class MyFaktory : Core.Faktory
    {
        string _buildDir = SourcePath + @"src\build\";
        string _stageDir = SourcePath + @"src\bin\";
        string _solutionPath = SourcePath + @"src\MsBuildTest.sln";

        protected override void Configure()
        {
            Config.Set("MSBuildPath", @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\amd64\MSBuild.exe");
        }

        protected override void RunBuild()
        {
            Requires("version")
                .Run(Logo)
                .Then(Clean)
                .Then(Stage)
                .Then(Build)
                .Then(Sign)
                .Execute();
        }

        void Logo()
        {
            Log("Faktory Sample", LogColor.Yellow);
        }

        void Clean()
        {
            Log("I am cleaning...");
            CleanDirectory(_buildDir);
            CleanDirectory(_stageDir);
            MsBuild.Clean(_solutionPath, "Release", "Any CPU");
        }

        void Build()
        {
            Log("I am building...");
            Thread.Sleep(300);
            MsBuild.Run(_solutionPath, "Release", "Any CPU");
        }

        void Stage()
        {
            Log("I am staging...");
            Thread.Sleep(300);
        }

        void Sign()
        {
            Log("I am signing...");
        }
    }
}
