namespace IsoNet.Core.IO.Codec;

public class StringCodec : AbstractTextCodec
{
    public static readonly StringCodec Instance = new();
    
    protected override void Write(object? item, TextWriter writer)
    {
        writer.Write(item);
    }

    protected override object? Read(TextReader reader, Type type)
    {
        return reader.ReadToEnd();
    }
}
