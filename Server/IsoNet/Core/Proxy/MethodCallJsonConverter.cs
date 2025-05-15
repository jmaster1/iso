using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IsoNet.Core.Proxy;

public class MethodCallJsonConverter : JsonConverterGeneric<MethodCall>
{
    public static readonly MethodCallJsonConverter Instance = new();
    
    protected override void WriteJson(JsonWriter writer, MethodCall value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("type");
        writer.WriteValue(value.MethodInfo.ReflectedType!.AssemblyQualifiedName);
        writer.WritePropertyName("method");
        writer.WriteValue(value.MethodInfo.Name);
        writer.WritePropertyName("args");
        serializer.Serialize(writer, value.Args);
        writer.WritePropertyName("attrs");
        serializer.Serialize(writer, value.Attrs);
        writer.WriteEndObject();
    }

    protected override MethodCall? ReadJson(JsonReader reader, MethodCall? value, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;
        
        var jo = JObject.Load(reader);
        var typeName = jo["type"]?.ToString();
        var methodName = jo["method"]?.ToString();
        var argsToken = jo["args"];
        var attrsToken = jo["attrs"];
        
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

        var mc = new MethodCall
        {
            MethodInfo = method, Args = args,
            AttrGetter = (name, type, defaultValue) =>
            {
                if (attrsToken is not JObject attrsObj)
                    return defaultValue;
                var token = attrsObj[name];
                return token != null && token.Type != JTokenType.Null
                    ? token.ToObject(type, serializer)
                    : defaultValue;
            }
                
        };
        return mc;
    }
}
