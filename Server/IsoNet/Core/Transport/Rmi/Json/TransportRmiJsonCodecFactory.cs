using IsoNet.Core.IO.Codec;

namespace IsoNet.Core.Transport.Rmi.Json;

public static class TransportRmiJsonCodecFactory
{
    public static JsonCodec Codec = CreateCodec();

    public static JsonCodec CreateCodec(params Type[] knownRmiTypes)
    {
        var methodCallJsonConverter = MethodCallJsonConverter.Instance;
        
        if (knownRmiTypes.Length > 0)
        {
            methodCallJsonConverter = new MethodCallJsonConverter
            {
                TypeStringConverter = new FastTypeStringConverter(knownRmiTypes)
            };
        }
        
        return new JsonCodec()
            .AddConverter(methodCallJsonConverter)
            .AddConverter(ExceptionJsonConverter.Instance)
            .AddConverter(VoidJsonConverter.Instance);
    }
}
