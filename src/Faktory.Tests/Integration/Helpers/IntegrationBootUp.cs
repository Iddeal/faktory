using Faktory.Core;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers
{
    [SetUpFixture]
    public class IntegrationBootUp
    {
        [OneTimeSetUp]
        public void Init()
        {
            FaktoryRunner.BootUp("", s => TestContext.Progress.WriteLine(s));
        }
    }
}