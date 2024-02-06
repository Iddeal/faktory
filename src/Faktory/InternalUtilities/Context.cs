using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faktory.Core.InternalUtilities
{
    internal static class Context
    {
        public enum CiRunners
        {
            None,
            TeamCity
        }

        public static CiRunners CiRunner =>
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")) == false 
                ? CiRunners.TeamCity 
                : CiRunners.None;
    }
}
