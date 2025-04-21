namespace IsoNet.Core.IO.Codec;

public class StringCodec : AbstractTextCodec<string>
{
    protected override void Write(string item, TextWriter writer)
    {
        writer.Write(item);
    }

    protected override string Read(TextReader reader)
    {
        return reader.ReadToEnd();
    }
}
