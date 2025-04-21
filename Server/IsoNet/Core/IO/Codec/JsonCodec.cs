using Newtonsoft.Json;

namespace IsoNet.Core.IO.Codec;

public class JsonCodec<T> : AbstractTextCodec<T>
{
    public JsonSerializer Serializer = JsonSerializer.CreateDefault();

    protected override void Write(T item, TextWriter writer)
    {
        using var jsonWriter = new JsonTextWriter(writer);
        Serializer.Serialize(jsonWriter, item);
        jsonWriter.Flush();
    }

    protected override T Read(TextReader reader)
    {
        using var jsonReader = new JsonTextReader(reader);
        return Serializer.Deserialize<T>(jsonReader)!;
    }
}
