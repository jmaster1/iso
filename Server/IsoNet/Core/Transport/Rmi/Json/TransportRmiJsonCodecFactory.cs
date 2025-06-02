using IsoNet.Core.IO.Codec;

namespace IsoNet.Core.Transport.Rmi.Json;

public static class TransportRmiJsonCodecFactory
{
    public static JsonCodec Codec = CreateCodec();
    
    public static JsonCodec CreateCodec()
    {
        return new JsonCodec()
            .AddConverter(MethodCallJsonConverter.Instance)
            .AddConverter(ExceptionJsonConverter.Instance)
            .AddConverter(VoidJsonConverter.Instance);
    }
}
