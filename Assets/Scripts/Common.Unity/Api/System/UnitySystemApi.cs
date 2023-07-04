using System;
using Common.Api.System;
using Common.IO.Streams;
using Common.Util.Http;
using Common.Util.Ntp;
using UnityEngine;
using UnityEngine.Profiling;

namespace Common.Unity.Api.System
{
    public class UnitySystemApi : SystemApi
    {
        /// <summary>
        /// Returns true if device is capable of connecting to internet
        /// 
        /// Note: Do not use this property to determine the actual connectivity.
        /// E.g. the device can be connected to a hot spot, but not have the actual route to the network.
        /// Non-handhelds are considered to always be capable of NetworkReachability.ReachableViaLocalAreaNetwork.
        /// </summary>
        public override bool IsNetworkConnected()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public override string GetLanguage()
        {
            return Application.systemLanguage.ToString();
        }

        public override string GetPlatform()
        {
            return Application.platform.ToString();
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            base.OnHttpResponse(query, html);
            html.h3("System Info");
            html.p("");
            html.p("Version: " + Application.version);
            html.p("FPS: " + (int) Mathf.Round(1.0f / UnityEngine.Time.unscaledDeltaTime));
            html.p("Device: " + SystemInfo.deviceModel);
            html.p("OS: " + SystemInfo.operatingSystem);
            html.p("CPU: " + SystemInfo.processorFrequency + " MHz (" + SystemInfo.processorCount + " cores)");
            html.p("RAM: " + SystemInfo.systemMemorySize + "MB");
            html.p("GPU: " + SystemInfo.graphicsDeviceName + ", " + SystemInfo.graphicsDeviceVersion);
            html.p("VRAM: " + SystemInfo.graphicsMemorySize + "MB");
            html.p(text: "Reserved: " + Profiler.GetTotalReservedMemoryLong() / (1024 * 1024) + "MB");
            html.p("Allocated: " + Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024) + "MB");
            html.p("Battery: " + SystemInfo.batteryLevel * 100 + "%");
            html.p("Network: " + Application.internetReachability);
            string netTime = null;
            try
            {
                netTime = NtpClient.GetNetworkTime().ToString();
            }
            catch (Exception ex)
            {
                netTime = ex.ToString();
            }
            html.p("network time: " + netTime);
        }
    }
}
