using System;
using Common.Api;
using Common.IO.Serialize;
using Common.Unity.Util;
using UnityEngine;

namespace Common.Unity.Api.AppRun
{
    /// <summary>
    /// provides info about app version update/run 
    /// </summary>
    public class AppRunApi : AbstractApi
    {
        public AppRunInfo appRunInfo;
        
        /// <summary>
        /// current version name
        /// </summary>
        public string Version => Application.version;

        public AppRunApi()
        {
            //
            // load info
            appRunInfo = NewtonsoftJsonObjectSerializer.Instance.Read<AppRunInfo>(UnityHelper.PrivateFileSystem);
            if(appRunInfo == null) appRunInfo = new AppRunInfo();
            //
            // register new run
            VersionRunInfo versionRunInfo = appRunInfo.versions.Find(Version);
            if (versionRunInfo == null)
            {
                versionRunInfo = new VersionRunInfo();
                versionRunInfo.versionName = Version;
                versionRunInfo.installed = DateTime.Now;
                appRunInfo.versions[Version] = versionRunInfo;
            }
            versionRunInfo.runCount++;
            NewtonsoftJsonObjectSerializer.Instance.Write(appRunInfo, UnityHelper.PrivateFileSystem);
        }

        /// <summary>
        /// check if this is first run of application
        /// </summary>
        public bool IsFirstRun()
        {
            if (appRunInfo.versions.Count > 1) return false;
            return IsFirstVersionRun();
        }
        
        /// <summary>
        /// check if this is first run of application current version
        /// </summary>
        public bool IsFirstVersionRun()
        {
            VersionRunInfo versionRunInfo = appRunInfo.versions[Version];
            return versionRunInfo.runCount == 1;
        }
    }
}