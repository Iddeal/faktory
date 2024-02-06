using System;
using Faktory.Core;
using Faktory.Core.ProgressReporter;
using NUnit.Framework;

namespace Faktory.Tests
{
    [TestFixture]
    public class FaktoryRunnerTests
    {
        private static readonly Action<string> UpdateStatus = s => TestContext.Progress.WriteLine(s);

        [Test]
        [NonParallelizable]
        public void WithAProgressReporter_ReportsEachStep()
        {
            var progressReporter = new TestProgressReporter();
            FaktoryRunner.ProgressReporter = progressReporter;
            FaktoryRunner.BootUp("", UpdateStatus);
            var faktory = new TestFaktoryWithTwoBuildSteps();
            faktory.SetStatusUpdater(UpdateStatus);
            FaktoryRunner.Run(faktory);

            CollectionAssert.AreEqual(new[]{ "Starting First", "Ending First", "Starting Second", "Ending Second"}, progressReporter.AllMessages);
        }

        [Test]
        [NonParallelizable]
        public void WhenAFailureOccurs_ReportsAFailure()
        {
            var progressReporter = new TestProgressReporter();
            FaktoryRunner.ProgressReporter = progressReporter;
            FaktoryRunner.BootUp("", UpdateStatus);
            var faktory = new TestFaktoryWithAFailingSecondStep();
            faktory.SetStatusUpdater(UpdateStatus);
            FaktoryRunner.Run(faktory);

            CollectionAssert.AreEqual(new[]{ "Starting First", "Ending First", "Starting Second", "Failed Second failed with - Failure"}, progressReporter.AllMessages);
        }
    }
}