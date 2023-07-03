using System;
using Common.TimeNS;
using Common.Util;
using Newtonsoft.Json;

namespace Common.IO.Serialize.Newtonsoft.Json.Converter
{
    /// <summary>
    /// TimeTask json converter
    /// </summary>
    public class TimeTaskConverter : JsonConverterGeneric<TimeTask>
    {
        private const string RunTime = "RunTime";
        
        private const string Duration = "Duration";
        
        private const string PausedTimeLeft = "PausedTimeLeft";

        protected override void WriteJson(JsonWriter writer, TimeTask value, JsonSerializer serializer)
        {
            LangHelper.Validate(value.External);
            if (value.Scheduled)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(RunTime);
                serializer.Serialize(writer, value.RunTime);
                writer.WritePropertyName(Duration);
                serializer.Serialize(writer, value.Duration);
                writer.WriteEndObject();
            } else if (value.Paused)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(PausedTimeLeft);
                serializer.Serialize(writer, value.PausedTimeLeft);
                writer.WritePropertyName(Duration);
                serializer.Serialize(writer, value.Duration);
                writer.WriteEndObject();
            } else writer.WriteNull();
        }

        protected override TimeTask ReadJson(JsonReader reader, TimeTask value, JsonSerializer serializer)
        {
            LangHelper.Validate(value != null);
            LangHelper.Validate(value.External);
            if (reader.IsNull()) return value;
            LangHelper.Validate(reader.IsStartObject());
            reader.Read();
            DateTime runTime = default;
            TimeSpan duration = default;
            TimeSpan pausedTimeLeft = default;
            while (reader.IsPropertyName())
            {
                var name = (string) reader.Value;
                reader.Read();
                switch (name)
                {
                    case RunTime:
                        runTime = serializer.Deserialize<DateTime>(reader);
                        break;
                    case Duration:
                        duration = serializer.Deserialize<TimeSpan>(reader);
                        break;
                    case PausedTimeLeft:
                        pausedTimeLeft = serializer.Deserialize<TimeSpan>(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                reader.Read();
            }

            if (runTime != default)
            {
                value.Schedule(runTime, duration);
            } else if (pausedTimeLeft != default)
            {
                value.ScheduleAfter(pausedTimeLeft, duration);
                value.Pause();
            }
            LangHelper.Validate(reader.IsEndObject());
            return value;
        }
    }
}