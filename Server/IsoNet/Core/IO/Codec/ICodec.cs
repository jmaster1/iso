namespace IsoNet.Core.IO.Codec;

public interface ICodec
{
    void Write(object? item, Stream target);

    T? Read<T>(Stream source)
    {
        return (T?) Read(source, typeof(T));
    }
    
    object? Read(Stream source, Type type);
}
