using System;
using Common.Lang.Entity;

namespace Common.Unity.Util.Submit
{
    /// <summary>
    /// report about exception with system metadata
    /// </summary>
    public class ErrorReport : AbstractEntity
    {
        /// <summary>
        /// system timestamp
        /// </summary>
        public DateTime systemTime;

        /// <summary>
        /// in-game time (seconds since app start)
        /// </summary>
        public float gameTime;

        /// <summary>
        /// debug flag
        /// </summary>
        public bool debug;

        /// <summary>
        /// application version
        /// </summary>
        public string appVersion;

        public string appId;
        
        public bool appGenuine;
        
        public string appPlatform;

        public string appInstaller;

        public string appInstallMode;
        
        public string systemLang;
        
        public string deviceModel;
        
        public string operatingSystem;
        
        public string deviceUid;
        
        public int systemMemorySizeMb;

        public string errorType;
        
        public string errorCondition;
        
        public string errorStacktrace;
    }
}