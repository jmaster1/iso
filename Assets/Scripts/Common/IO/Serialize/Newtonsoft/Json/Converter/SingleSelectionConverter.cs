using System;
using Common.Lang.Selection;
using Newtonsoft.Json;

namespace Common.IO.Serialize.Newtonsoft.Json.Converter
{
    /// <summary>
    /// SingleSelection json converter, serializes selected object
    /// </summary>
    public class SingleSelectionConverter<T> : JsonConverterGeneric<SingleSelection<T>> where T : class
    {
        protected override void WriteJson(JsonWriter writer, SingleSelection<T> value, JsonSerializer serializer)
        {
            var selected = value.Selected.Get();
            serializer.Serialize(writer, selected);
        }

        protected override SingleSelection<T> ReadJson(JsonReader reader, SingleSelection<T> value, JsonSerializer serializer)
        {
            var selected = serializer.Deserialize<T>(reader);
            value.Select(selected);
            return value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SingleSelection<T>);
        }
    }
}