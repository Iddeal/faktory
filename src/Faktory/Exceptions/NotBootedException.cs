using System;

namespace Faktory.Exceptions
{
    public class NotBootedException : Exception
    {
        public override string Message => "Ensure Faktory.Boot.Up() is called first.";
    }
}