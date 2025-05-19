namespace IsoNet.Core.Transport.Rmi;

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ExceptionJsonConverter : JsonConverter<Exception>
{
    public static readonly ExceptionJsonConverter Instance = new();
    
    public override void WriteJson(JsonWriter writer, Exception? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Type");
        writer.WriteValue(value?.GetType().FullName);
        writer.WritePropertyName("Message");
        writer.WriteValue(value?.Message);
        writer.WriteEndObject();
    }

    public override Exception? ReadJson(JsonReader reader, Type objectType, Exception? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;
        
        var obj = JObject.Load(reader);

        var typeName = (string?)obj["Type"];
        var message = (string?)obj["Message"];

        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        try
        {
            var type = Type.GetType(typeName);
            if (type == null || !typeof(Exception).IsAssignableFrom(type))
                return new Exception(message);

            return (Exception?)Activator.CreateInstance(type, message);
        }
        catch
        {
            return new Exception(message);
        }
    }

    public override bool CanWrite => true;
    public override bool CanRead => true;
}
