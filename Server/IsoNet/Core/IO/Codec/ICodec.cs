namespace IsoNet.Core.IO.Codec;

public interface ICodec<T>
{
    void Write(T item, Stream target);
    
    T Read(Stream source); 
}
