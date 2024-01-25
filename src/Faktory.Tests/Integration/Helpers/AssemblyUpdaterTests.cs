using System;
using System.IO;
using Faktory.Core.Helpers;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class AssemblyUpdaterTests
    {
        const string assemblyInfoPath = @"..\..\..\..\MsBuildTest\Properties\AssemblyInfo.cs";

        [Test, Order(1)]
        [NonParallelizable]
        public void Update_NonExistentFile_Fails()
        {
            Assert.Throws<Exception>(() => AssemblyUpdater.Update("I-dont-exist.nada", AssemblyUpdater.AssemblyInfo.Default()));
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void Update_ValidFile_Succeeds()
        {

            // Arrange 
            var before = File.ReadAllText(assemblyInfoPath);
            var attributes = new AssemblyUpdater.AssemblyInfo()
            {
                Title = "My Title",
                Description = "My Description",
                Company = "My Company",
                Product = "My Product",
                Copyright = "My Copyright",
                Trademark = "My Trademark",
                Version = "My Version",
                FileVersion = "My FileVersion"
            };

            // Act - Clean the path
            AssemblyUpdater.Update(assemblyInfoPath, attributes);
            var after = File.ReadAllText(assemblyInfoPath);

            // Reset the original file
            File.WriteAllText(assemblyInfoPath, before);

            // Assert Before State
            StringAssert.Contains("AssemblyTitle(\"MsBuildTest\")", before);
            StringAssert.Contains("AssemblyDescription(\"\")", before);
            StringAssert.Contains("AssemblyConfiguration(\"\")", before);
            StringAssert.Contains("AssemblyCompany(\"\")", before);
            StringAssert.Contains("AssemblyProduct(\"MsBuildTest\")", before);
            StringAssert.Contains("AssemblyCopyright(\"Copyright ©  2024\")", before);
            StringAssert.Contains("AssemblyTrademark(\"\")", before);
            StringAssert.Contains("AssemblyVersion(\"1.0.0.0\")", before);
            StringAssert.Contains("AssemblyFileVersion(\"1.0.0.0\")", before);

            // Assert After State
            StringAssert.Contains("AssemblyTitle(\"My Title\")", after);
            StringAssert.Contains("AssemblyDescription(\"My Description\")", after);
            StringAssert.Contains("AssemblyConfiguration(\"\")", after);
            StringAssert.Contains("AssemblyCompany(\"My Company\")", after);
            StringAssert.Contains("AssemblyProduct(\"My Product\")", after);
            StringAssert.Contains("AssemblyCopyright(\"My Copyright\")", after);
            StringAssert.Contains("AssemblyTrademark(\"My Trademark\")", after);
            StringAssert.Contains("AssemblyVersion(\"My Version\")", after);
            StringAssert.Contains("AssemblyFileVersion(\"My FileVersion\")", after);
        }
    }
}