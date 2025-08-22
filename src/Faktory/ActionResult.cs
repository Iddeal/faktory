using System;
using System.Collections.Generic;

namespace Faktory.Core
{
    public class ActionResult
    {
        public const int IndentWidth = 4;

        public string Name { get; set; } 
        public TimeSpan Duration { get; set; } 
        public Exception LastException { get; set; } 
        public bool Success => LastException == null;
        public List<string> Messages { get; } = [];

        public void AddMessage(string message = "", int indent = 0) => Messages.Add($"{new string(' ', indent * IndentWidth)}{message}");
    }
}