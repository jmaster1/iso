using Common.Util;
using Newtonsoft.Json;

namespace Common.IO.Serialize.Newtonsoft.Json.Converter
{
    public class NonAddingArrayConverter<TArray, TElement> : JsonConverterGeneric<TArray>
    {

        public override bool CanWrite => false;

        protected override void WriteJson(JsonWriter writer, TArray value, JsonSerializer serializer)
        {
        }

        protected override TArray? ReadJson(JsonReader reader, TArray? value, JsonSerializer serializer)
        {
            LangHelper.Validate(reader.IsStartArray());
            while (!reader.IsEndArray())
            {
                reader.Read();
                if (reader.IsStartObject())
                {
                    serializer.Deserialize<TElement>(reader);
                }
            }
            return value;
        }
    }
}
