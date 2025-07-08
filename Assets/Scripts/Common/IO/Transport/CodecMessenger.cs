using System;
using Common.IO.Codec;

namespace IsoNet.Core.Transport
{
    public class CodecMessenger<T>
    {

        private readonly AbstractTransport transport;
        private readonly ICodec codec;
        private readonly Action<T> handler;

        public CodecMessenger(AbstractTransport transport, ICodec codec, Action<T> handler)
        {
            this.transport = transport;
            this.codec = codec;
            this.handler = handler;
        }

        public CodecMessenger<T> Init()
        {
            transport.SetMessageHandler(stream =>
            {
                var message = codec.Read<T>(stream);
                handler(message!);
            });
            return this;
        }
    
        public void SendMessage(T message)
        {
            transport.SendMessage(stream => codec.Write(message, stream));
        }
    }
}
