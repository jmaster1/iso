namespace IsoNet.Core.IO.Codec;

public interface ICodec2
{
    void Write<T>(T item, Stream target);
    
    T Read<T>(Stream source); 
}
