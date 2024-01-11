using System.Linq;
using Faktory.Core;
using NUnit.Framework;

namespace Faktory.Tests
{
    [TestFixture]
    public class OptionsTests
    {
        [Test]
        public void WithNoOptions_IsOk()
        {
            var options = new Options("");

            Assert.True(options.Valid);
        }

        [Test]
        public void InvalidFormat_FailsWithError()
        {
            var options = new Options("--invalid");

            Assert.False(options.Valid);
            Assert.IsNotEmpty(options.InvalidOptions);
            Assert.True(options.InvalidOptions.Any(x => x.Key == "--invalid"));
        }

        [Test]
        public void MissingRightHandValue_FailsWithError()
        {
            var options = new Options("value=");

            Assert.False(options.Valid);
            Assert.IsNotEmpty(options.InvalidOptions);
            Assert.True(options.InvalidOptions.Any(x => x.Key == "value"));
        }

        [Test]
        public void WithLotsOfExtraPadding_IsOk()
        {
            var options = new Options("value  =\"  SomeValue   \"");

            Assert.True(options.Valid);
            Assert.True(options.HasAll(new[] { "value" }.ToList()));
            Assert.AreEqual(options["value"], "  SomeValue   ");
        }
    }
}
