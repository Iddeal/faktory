namespace Faktory
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
        public int? ExitCode { get; private set; }

        public ProcessStepResult(Status status, string message, int? exitCode = null) : base(status, message)
        {
            ExitCode = exitCode;
        }
    }
}