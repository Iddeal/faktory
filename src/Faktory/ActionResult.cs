using System;

namespace Faktory.Core
{
    public struct ActionResult
    {
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public Exception LastException { get;  set; }
        public bool Success => LastException != null;
    }
}