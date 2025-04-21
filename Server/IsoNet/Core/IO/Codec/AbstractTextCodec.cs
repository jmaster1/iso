namespace IsoNet.Core.IO.Codec;

public abstract class AbstractTextCodec<T> : ICodec<T>
{
    public void Write(T item, Stream target)
    {
        using var writer = new StreamWriter(target, leaveOpen: true);
        Write(item, writer);
    }

    protected abstract void Write(T item, TextWriter writer);

    public T Read(Stream source)
    {
        using var reader = new StreamReader(source, leaveOpen: true);
        return Read(reader);
    }

    protected abstract T Read(TextReader reader);
}
