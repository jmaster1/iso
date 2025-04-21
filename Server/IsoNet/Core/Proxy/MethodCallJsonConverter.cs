using IsoNet.Core.IO.Codec;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IsoNet.Core.Proxy;

public class MethodCallJsonConverter : JsonConverter
{
    public static JsonCodec<MethodCall> Codec { get; } = CreateCodec();
    public static JsonCodec<MethodCall> CreateCodec()
    {
        return new JsonCodec<MethodCall>
        {
            Serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new MethodCallJsonConverter()
                },
                Formatting = Formatting.None,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            })
        };
    }
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not MethodCall custom)
        {
            throw new JsonSerializationException("value is not MethodCall");
        }
        writer.WriteStartObject();
        writer.WritePropertyName("type");
        writer.WriteValue(custom.MethodInfo.ReflectedType!.AssemblyQualifiedName);
        writer.WritePropertyName("method");
        writer.WriteValue(custom.MethodInfo.Name);
        writer.WritePropertyName("args");
        serializer.Serialize(writer, custom.Args);
        writer.WriteEndObject();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;
        
        var jo = JObject.Load(reader);
        var typeName = jo["type"]?.ToString();
        var methodName = jo["method"]?.ToString();
        var argsToken = jo["args"];

        if (typeName == null || methodName == null || argsToken == null)
            throw new JsonSerializationException("Missing fields in MethodCall JSON.");

        var targetType = Type.GetType(typeName)
                         ?? throw new JsonSerializationException($"Type '{typeName}' not found");
        
        var method = targetType.GetMethod(methodName);
        if (method == null)
            throw new JsonSerializationException($"Method '{methodName}' not found in type '{targetType}'");
        
        var parameters = method.GetParameters();
        var args = new object?[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            args[i] = argsToken[i]?.ToObject(parameters[i].ParameterType, serializer);
        }
        
        return new MethodCall
        {
            MethodInfo = method,
            Args = args
        };
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableFrom(typeof(MethodCall));
    }
}
