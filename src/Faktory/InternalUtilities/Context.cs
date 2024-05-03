using System;

namespace Faktory.Core.InternalUtilities
{
    internal static class Context
    {
        public static CiRunners CiRunner =>
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")) == false 
                ? CiRunners.TeamCity 
                : CiRunners.None;
    }
}
