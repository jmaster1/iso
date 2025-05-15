namespace IsoNet.Core.IO.Codec;

public abstract class AbstractTextCodec : ICodec
{
    protected abstract void Write(object? item, TextWriter writer);

    protected abstract object? Read(TextReader reader, Type type);
    
    public void Write(object? item, Stream target)
    {
        using var writer = new StreamWriter(target, leaveOpen: true);
        Write(item, writer);
    }

    public object? Read(Stream source, Type type)
    {
        using var reader = new StreamReader(source, leaveOpen: true);
        return Read(reader, type);
    }
}
