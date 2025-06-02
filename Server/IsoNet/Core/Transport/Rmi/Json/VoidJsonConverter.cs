using Newtonsoft.Json;

namespace IsoNet.Core.Transport.Rmi.Json;

public class VoidJsonConverter : JsonConverter
{
    public static readonly VoidJsonConverter Instance = new();
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(void);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        while (reader.TokenType == JsonToken.Comment)
            reader.Read();

        if (reader.TokenType != JsonToken.Null)
            throw new JsonSerializationException($"Cannot convert {reader.TokenType} to System.Void.");

        return null!;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteNull();
    }

    public override bool CanRead => true;
    public override bool CanWrite => true;
}