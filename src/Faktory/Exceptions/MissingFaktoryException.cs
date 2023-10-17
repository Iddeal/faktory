using System;

namespace Faktory.Exceptions
{
    public class MissingFaktoryException : Exception
    {
        public override string Message => "Unable to find implementation of Faktory.";
    }
}