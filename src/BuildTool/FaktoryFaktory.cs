using Faktory.Core;
using Faktory.Core.Helpers;
using Faktory.Core.Logging;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BuildTool
{
    // Note: This project isn't built by default. You need to manually build the Debug configuration to use build.bat.
    public class FaktoryFaktory : Faktory.Core.Faktory
    {
        string _rootDir;
        string _srcDir;
        string _packagesDir;
        string _solutionPath;
        string _nugetPath;
        string _buildConfiguration;
        string[] _testPaths;
        string _testsDir;

        static string MsBuildPath => $@"C:\Program Files\Microsoft Visual Studio\2022\{VisualStudioEdition}\MSBuild\Current\Bin\amd64\MSBuild.exe";
        static string VisualStudioEdition =>
            Directory.Exists(@"C:\Program Files\Microsoft Visual Studio\2022\Professional")
                ? "Professional"
                : "Enterprise";

        protected override void Configure()
        {
            var tasks = new[] { "release", "test" };

            if (Options["task"] == null || !tasks.Contains(Options["task"].ToLowerInvariant()))
            {
                Fail($"Option 'task' must be one of [{string.Join(",", tasks)}]");
            }

            // If task=test then we're use debug not release
            _buildConfiguration = Options["task"].ToLowerInvariant() == "test" ? "Debug" : "Release";

            // Paths
            _rootDir = new DirectoryInfo(Path.Combine(SourcePath, @"..\..\..\..\")).FullName;
            _srcDir = _rootDir + @"src\";
            _packagesDir = _srcDir + "packages";
            _solutionPath = _srcDir + "Faktory.sln";
            _testsDir = _srcDir + @"\Faktory.Tests\bin\Debug\";
            _testPaths = [_testsDir + "Faktory.Tests.dll"];
            _nugetPath = _srcDir + @".nuget\nuget.exe";

            Config.Set("MSBuildPath", MsBuildPath);
            Config.Set("NUnitPath", NUnit.FindRunner(_packagesDir));
        }

        protected override void RunBuild()
        {
            switch (Options["task"]?.ToLower())
            {
                case "test":
                    Requires("task")
                        .Then(Clean)
                        .Then(RestoreNuget)
                        .Then(BuildApp)
                        .Then(Test)
                        .Execute();
                    break;
                case "release":
                    Requires("task", "version")
                        .Run(Logo)
                        .Then(Clean)
                        .Then(RestoreNuget)
                        .Then(UpdateAssemblyInfo)
                        .Then(UpdateNuspec)
                        .Then(BuildApp)
                        .Execute();
                    break;
                default:
                    Requires("task")
                        .Execute();
                    break;
            }
        }

        void Logo()
        {
            Log(@"Faktory", LogColor.Purple);
        }

        void Clean()
        {
            Io.CleanDirectory(_packagesDir);
            MsBuild.Clean(_solutionPath, _buildConfiguration);
        }

        void RestoreNuget()
        {
            Process.Run(_nugetPath, $"restore {_solutionPath}");
        }

        void UpdateAssemblyInfo()
        {
            var path = Path.Combine(_srcDir, @"Faktory\Properties\AssemblyInfo.cs");
            var assemblyInfo = new AssemblyUpdater.AssemblyInfo()
            {
                Title = "Faktory",
                Description = "Insanely simple build tool",
                Version = Options["version"],
                FileVersion = Options["version"]
            };
            Log($"Updating {path}..", LogColor.Yellow);
            AssemblyUpdater.Update(path, assemblyInfo);
        }

        void UpdateNuspec()
        {
            var path = Path.Combine(_srcDir, @"Faktory\Faktory.Core.nuspec");
            var doc = XDocument.Load(path);
            XNamespace ns = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd";
            var versionElement = doc.Element(ns + "package")?.Element(ns + "metadata")?.Element(ns + "version");
            if (versionElement != null)
            {
                versionElement.Value = Options["version"];
                doc.Save(path);
                Log($"Version updated to {Options["version"]}", LogColor.Green);
            }
            else
            {
                Log("Version element not found in the nuspec file", LogColor.Yellow);
            }
        }

        void BuildApp()
        {
            MsBuild.Run(_solutionPath, _buildConfiguration, optimize: true);
        }

        void Test()
        {
            NUnit.RunTests(_testPaths, _rootDir);
            MSpec.RunTests(_testPaths, _rootDir);
        }
    }
}