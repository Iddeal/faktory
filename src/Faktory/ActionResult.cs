using System;

namespace Faktory.Core
{
    public struct ActionResult
    {
        public string Name { get; set; }
        public bool Success { get; set; }
        public TimeSpan Duration { get; set; }
    }
}