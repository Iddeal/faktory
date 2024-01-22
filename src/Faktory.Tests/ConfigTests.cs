using Faktory.Core;
using NUnit.Framework;

namespace Faktory.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void SettingConfig_Succeeds()
        {
            // Arrange
            var config = new Config();

            // Act
            config["myKey"] = "MyValue";

            Assert.AreEqual("MyValue", config["myKey"]);
        }
    }
}