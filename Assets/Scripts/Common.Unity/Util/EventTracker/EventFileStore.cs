using System;
using System.Collections.Generic;
using System.IO;
using Common.IO.Serialize.Newtonsoft.Json;
using Common.Lang;
using Common.Lang.Entity;
using Common.Util;
using Common.Util.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Unity.Util.EventTracker
{
    /// <summary>
    /// store events as json (1 event = 1 line) in a file
    /// convert to json and write to file in async thread
    /// </summary>
    public class EventFileStore : GenericBean
    {
        ThreadedTaskQueue Queue = new ThreadedTaskQueue(1, "EventFileStore-");

        /// <summary>
        /// path to file
        /// </summary>
        public string path;
        
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter()
            },
            Formatting = Formatting.None,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };
        
        JsonSerializer serializer;
        
        public void WriteEvent(Dictionary<string, object> map)
        {
            LangHelper.Validate(map != null);
            LangHelper.Validate(map.Count > 0);
            Queue.PushTask(() =>
            {
                if (serializer == null) serializer = JsonSerializer.CreateDefault(settings);
                string line = serializer.ToJson(map);
                using (StreamWriter fileWriter = File.AppendText(path))
                {
                    fileWriter.WriteLine(line);
                    fileWriter.Flush();
                }
            });
        }

        public void MoveFile(string target, Action action)
        {
            Queue.PushTask(() =>
            {
                try
                {
                    File.Move(path, target);
                    action();
                }
                catch (Exception ex)
                {
                    Log.Warn("Failed to move {path} > {target}");
                }
            });
        }

        public bool FileExists()
        {
            return File.Exists(path);
        }
    }
}