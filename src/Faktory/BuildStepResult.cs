using System.Collections.Generic;

namespace Faktory.Core
{
    public enum Status
    {
        Ok,
        Error
    }

    public class BuildStepResult
    {
        public static BuildStepResult Success(string message = null) => new(Status.Ok, message);
        public static BuildStepResult Error(string message = null) => new(Status.Error, message);

        public Status Status { get; private set; }
        public string Message { get; private set; }

        public BuildStepResult(Status status, string message)
        {
            Status = status;
            Message = message;
        }
    }

    public class ProcessStepResult : BuildStepResult
    {
        public List<string> StandardOut { get; }
        public List<string> StandardError { get; }
        public int? ExitCode { get; private set; }

        public ProcessStepResult(Status status, string message, List<string> standardOut = null, List<string> standardError = null, int? exitCode = null) : base(status, message)
        {
            StandardOut = standardOut;
            StandardError = standardError;
            ExitCode = exitCode;
        }
    }
}