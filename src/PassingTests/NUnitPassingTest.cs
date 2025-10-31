using NUnit.Framework;

namespace PassingTestsTestProject
{
    [TestFixture]
    public class NUnitPassingTest
    {
        [Test]
        public void Test()
        {
            Assert.Pass();
        }
    }
}
