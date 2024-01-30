using System;

namespace Faktory.Core.Exceptions
{
    public class MissingFaktoryException : Exception
    {
        public override string Message => "Unable to find implementation of Faktory.";
    }

    public class BuildFailureException : Exception
    {
        public BuildFailureException(string message) : base(message)
        {
        }
    }
}