using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Common.Bind;
using Common.ContextNS;
using Common.IO.Streams;
using Common.TimeNS;
using Common.Util.Http;

namespace Common.Unity.Util.EventTracker
{
    /// <summary>
    /// responsible for buffering events to file and submitting file
    /// to server on time interval
    /// </summary>
    public class EventTracker : BindableBean<TaskManager>
    {
        EventTrackerInfo info = Context.GetInfo<EventTrackerInfo>();
        
        EventFileStore store = new EventFileStore();
        
        EventSender sender = new EventSender();

        TimeTask flushTask;
        
        string fileSend => store.path + ".tmp";

        /// <summary>
        /// prevent sending events to server
        /// </summary>
        public bool flushDisabled;
        
        protected override void OnBind()
        {
            store.path = Path.Combine(UnityHelper.PersistentPrivateDataPath, info.StoreFileName);
            if(Log.IsDebugEnabled) Log.Debug($"store.path={store.path}");
            //
            // check if need to flush existing file
            SendFile();
            ScheduleFlush();
        }

        protected override void OnUnbind()
        {
            flushTask = flushTask?.Cancel();
        }
        
        void ScheduleFlush()
        {
            if(flushDisabled) return;
            flushTask = Model.ScheduleAfterSec(Flush, info.FlushInterval);
            if(Log.IsDebugEnabled) Log.Debug($"scheduled flush: {flushTask}");
        }

        [HttpInvoke]
        public void Flush()
        {
            if(flushDisabled) return;
            if(Log.IsDebugEnabled) Log.Debug($"Flush begin");
            try
            {
                if (!store.FileExists() || File.Exists(fileSend))
                {
                    return;
                }
                store.MoveFile(fileSend, SendFile);
            }
            catch (Exception ex)
            {
                Log.Warn("Flush() failed", ex);
            }
            finally
            {
                ScheduleFlush();
            }
        }

        async void SendFile()
        {
            if(!File.Exists(fileSend)) return;
            try
            {
                if (Log.IsDebugEnabled) Log.Debug("sending file");
                HttpResponseMessage response = await sender.Send(fileSend, info);
                if (Log.IsDebugEnabled) Log.Debug($"response code: {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    if (Log.IsDebugEnabled) Log.Debug("deleting file");
                    File.Delete(fileSend);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("SendFile() failed", ex);
            }
        }

        public void TrackEvent(Dictionary<string, object> map)
        {
            store.WriteEvent(map);
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.propertyTable("flushTask", flushTask);
        }
    }
}