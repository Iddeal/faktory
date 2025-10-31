using NUnit.Framework;

namespace FailingTests
{
    [TestFixture]
    public class NUnitFailingTest
    {
        [Test]
        public void Test()
        {
            Assert.Fail();
        }
    }
}
