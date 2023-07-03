using System;
using Common.Lang;
using Common.Lang.Observable;
using Newtonsoft.Json;

namespace Common.IO.Serialize.Newtonsoft.Json.Converter
{
    /// <summary>
    /// HolderRaw json converter
    /// </summary>
    public class HolderConverter : JsonConverterGeneric<HolderRaw>
    {
        protected override void WriteJson(JsonWriter writer, HolderRaw value, JsonSerializer serializer)
        {
            var v = value.GetRaw();
            serializer.Serialize(writer, v);
        }

        protected override HolderRaw ReadJson(JsonReader reader, HolderRaw value, JsonSerializer serializer)
        {
            var type = value.GetValueType();
            var v = serializer.Deserialize(reader, type);
            value.SetRaw(v);
            return value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(HolderRaw).IsAssignableFrom(objectType);
        }
    }
}