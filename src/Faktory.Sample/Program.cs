using System;
using System.IO;
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
            Console.ReadLine();
        }
    }

    public class MyFaktory : Core.Faktory
    {
        string _rootDir;
        string _buildDir;
        string _stageDir;
        string _solutionPath;

        static string MsBuildPath => $@"C:\Program Files\Microsoft Visual Studio\2022\{VisualStudioEdition}\MSBuild\Current\Bin\amd64\MSBuild.exe";
        static string VisualStudioEdition =>
            Directory.Exists(@"C:\Program Files\Microsoft Visual Studio\2022\Professional")
                ? "Professional"
                : "Enterprise";

        protected override void Configure()
        {
            Config.Set("MSBuildPath", MsBuildPath);
            _rootDir = new DirectoryInfo(Path.Combine(SourcePath, @"..\..\..\..\..\")).FullName;
            _buildDir = Path.Combine(_rootDir, @"src\build\");
            _stageDir = Path.Combine(_rootDir, @"src\bin\");
            _solutionPath = Path.Combine(_rootDir, @"src\MsBuildTest\MsBuildTest.sln");
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
            MsBuild.Clean(_solutionPath, "Release", "x64");
        }

        void Build()
        {
            Log("I am building...");
            Thread.Sleep(300);
            MsBuild.Run(_solutionPath, "Release", "x64");
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
