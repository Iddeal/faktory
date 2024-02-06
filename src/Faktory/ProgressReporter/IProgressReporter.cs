using System;
using System.Collections.Generic;

namespace Faktory.Core.ProgressReporter
{
    public interface IProgressReporter
    {
        void ReportStartProgress(string methodName);
        void ReportEndProgress(string methodName);
        void ReportFailure(string message);

    }

    public class TeamCityProgressReporter : IProgressReporter
    {
        public void ReportStartProgress(string methodName) => Console.WriteLine($"##teamcity[progressStart '{methodName}']");
        public void ReportEndProgress(string methodName) => Console.WriteLine($"##teamcity[progressFinish '{methodName}']");
        public void ReportFailure(string message) => Console.WriteLine($"##teamcity[buildProblem description='{message}']");
    }

    public class NullProgressReporter : IProgressReporter
    {

        public void ReportStartProgress(string methodName) { }
        public void ReportEndProgress(string methodName) { }
        public void ReportFailure(string message) { }
    }

    public class TestProgressReporter : IProgressReporter
    {
        public List<string> AllMessages { get; } = new();

        public void ReportStartProgress(string methodName) => AllMessages.Add($"Starting {methodName}");
        public void ReportEndProgress(string methodName) => AllMessages.Add($"Ending {methodName}");
        public void ReportFailure(string message) => AllMessages.Add($"Failed {message}");
    }


}
