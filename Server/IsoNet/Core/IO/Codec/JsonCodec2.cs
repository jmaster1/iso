using Newtonsoft.Json;

namespace IsoNet.Core.IO.Codec;

public class JsonCodec2 : AbstractTextCodec2
{
    public JsonSerializer Serializer = JsonSerializer.CreateDefault();

    protected override void Write(object? item, TextWriter writer)
    {
        using var jsonWriter = new JsonTextWriter(writer);
        try
        {
            Serializer.Serialize(jsonWriter, item);
            jsonWriter.Flush();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to write json for: {item}", e);
        }
    }

    protected override object? Read(TextReader reader, Type type)
    {
        using var jsonReader = new JsonTextReader(reader);
        return Serializer.Deserialize(jsonReader, type);
    }

    public JsonCodec2 AddConverter(JsonConverter converter)
    {
        Serializer.Converters.Add(converter);
        return this;
    }
}
