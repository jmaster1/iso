using IsoNet.Core.IO.Codec;

namespace IsoNet.Core.Transport;

public class CodecMessenger<T>(
    AbstractTransport transport,
    ICodec codec, 
    Action<T> handler)
{
    public CodecMessenger<T> Init()
    {
        transport.SetMessageHandler(stream =>
        {
            //var message = codec.Read(stream);
            ////handler(message);
        });
        return this;
    }
    
    public void SendMessage(T message)
    {
        transport.SendMessage(stream => codec.Write(message, stream));
    }
}
