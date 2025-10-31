using Machine.Specifications;

namespace FailingTests
{
    public class MSpecFailingTest
    {
        private It should_fail = () => false.ShouldBeTrue();
    }
}
