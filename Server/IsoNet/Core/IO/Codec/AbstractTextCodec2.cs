namespace IsoNet.Core.IO.Codec;

public abstract class AbstractTextCodec2 : ICodec2
{
    protected abstract void Write<T>(T item, TextWriter writer);

    protected abstract T Read<T>(TextReader reader);
    
    public void Write<T>(T item, Stream target)
    {
        using var writer = new StreamWriter(target, leaveOpen: true);
        Write(item, writer);
    }

    public T Read<T>(Stream source)
    {
        using var reader = new StreamReader(source, leaveOpen: true);
        return Read<T>(reader);
    }
}
