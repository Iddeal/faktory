using NUnit.Framework;

namespace FailingTestsTestProject
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
