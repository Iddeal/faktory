using Machine.Specifications;

namespace FailingTestsTestProject
{
    public class MSpecFailingTest
    {
        private It should_fail = () => false.ShouldBeTrue();
    }
}
