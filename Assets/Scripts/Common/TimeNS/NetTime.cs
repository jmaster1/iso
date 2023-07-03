using System;
using Common.Api.System;
using Common.ContextNS;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Observable;
using Common.Util.Http;
using Common.Util.Ntp;

namespace Common.TimeNS
{
    public class NetTime : Time
    {
        private SystemApi systemApi = Context.Get<SystemApi>();

        /// <summary>
        /// shows whether time is synchronized
        /// </summary>
        public readonly BoolHolder Sync = new BoolHolder();
        
        /// <summary>
        /// sync count
        /// </summary>
        public int SyncCount { get; private set; }
        
        /// <summary>
        /// last sync error
        /// </summary>
        public Exception SyncError { get; private set; }
        
        /// <summary>
        /// netTime - localTime
        /// </summary>
        public TimeSpan DeltaNetLocal { get; private set; }

        /// <summary>
        /// shows whether sync should be skipped (debug purposes)
        /// </summary>
        public bool SyncSkip;
        
        public override void Update()
        {
            if (systemApi.IsNetworkConnected())
            {
                if (!Sync.Get() && !SyncSkip)
                {
                    SyncError = null;
                    try
                    {
                        SyncCount++;
                        var netTime = NtpClient.GetNetworkTime();
                        DeltaNetLocal = netTime - DateTime.Now;
                        Sync.SetTrue();
                    }
                    catch (Exception ex)
                    {
                        SyncError = ex;
                    }
                }
            }
            else
            {
                Sync.SetFalse();
            }
            if (Sync.Get())
            {
                Delta = DeltaNetLocal;
                Value = DateTime.Now + DeltaNetLocal + Offset;
                Notify();
            }
        }

        /// <summary>
        /// force time synchronization on next update
        /// </summary>
        [HttpInvoke]
        public void ForceSync()
        {
            Sync.SetFalse();
        }
        
        [HttpInvoke]
        public void SyncSkipSet()
        {
            SyncSkip = true;
        }
        
        [HttpInvoke]
        public void SyncSkipReset()
        {
            SyncSkip = false;
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.propertyTable(
                "Value", Value,
                "Delta", Delta,
                "Sync", Sync,
                "SyncSkip", SyncSkip,
                "SyncCount", SyncCount,
                "SyncError", SyncError,
                "DeltaNetLocal", DeltaNetLocal);
        }
    }
}