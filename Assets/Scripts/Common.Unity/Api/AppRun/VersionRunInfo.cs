using System;

namespace Common.Unity.Api.AppRun
{
    
    /// <summary>
    /// container for app version run history (install/run count)
    /// </summary>
    public class VersionRunInfo
    {
        public string versionName;

        public DateTime installed;

        public int runCount;
    }
}
