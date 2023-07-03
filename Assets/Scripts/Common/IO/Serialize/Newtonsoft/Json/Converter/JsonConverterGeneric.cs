using System;
using Common.Lang;
using Common.Lang.Observable;
using Common.Util;
using Newtonsoft.Json;

namespace Common.IO.Serialize.Newtonsoft.Json.Converter
{
    /// <summary>
    /// JsonConverter extension with generic type
    /// </summary>
    public abstract class JsonConverterGeneric<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = (T) value;
            WriteJson(writer, t, serializer);
        }

        protected abstract void WriteJson(JsonWriter writer, T value, JsonSerializer serializer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ReadJson(reader, (T)existingValue, serializer);
        }

        protected abstract T ReadJson(JsonReader reader, T value, JsonSerializer serializer);

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
        
        /// <summary>
        /// write ObsListMapString elements as array
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        /// <param name="filter">optional filter that should accept element for serializing</param>
        /// <typeparam name="TE"></typeparam>
        protected void WriteObsListMap<TE>(JsonWriter writer, ObsListMapString<TE> value, JsonSerializer serializer,
            Func<TE, bool> filter = null) where TE : IIdAware<string>
        {
            writer.WriteStartArray();
            foreach (var e in value)
            {
                if(filter != null && !filter(e)) continue;
                writer.WriteStartObject();
                writer.WritePropertyName(e.GetId());
                serializer.Serialize(writer, e);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        /// <summary>
        /// read pre-populated ObsListMapString elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        /// <param name="elementCallback">optional callback to be invoked for each element after population</param>
        /// <param name="elementFactory"></param>
        /// <typeparam name="TE"></typeparam>
        /// <returns></returns>
        protected ObsListMapString<TE> ReadObsListMap<TE>(JsonReader reader, 
            ObsListMapString<TE> value, 
            JsonSerializer serializer, 
            Action<TE> elementCallback = null,
            Func<string, int, TE> elementFactory = null) where TE : IIdAware<string>
        {
            LangHelper.Validate(reader.IsStartArray());
            for (var i = 0;; i++)
            {
                reader.Read();
                if (reader.IsEndArray()) break;
                LangHelper.Validate(reader.IsStartObject());
                var id = reader.ReadPropertyName();
                var e = elementFactory == null ? value.GetByKey(id) : value.FindByKey(id);
                if (e == null && elementFactory != null)
                {
                    e = elementFactory(id, i);
                }
                reader.ReadStartObject();
                serializer.Populate(reader, e);
                reader.ReadEndObject();
                elementCallback?.Invoke(e);
            }
            return value;
        }
    }
}