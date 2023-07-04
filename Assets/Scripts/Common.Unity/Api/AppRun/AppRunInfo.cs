using System;
using Common.Lang;
using Common.Lang.Collections;
using UnityEngine;

namespace Common.Unity.Api.AppRun
{
    /// <summary>
    /// container for app run history (install/run count per version)
    /// </summary>
    public class AppRunInfo
    {
        public Map<string, VersionRunInfo> versions = new Map<string, VersionRunInfo>();
    }
}
