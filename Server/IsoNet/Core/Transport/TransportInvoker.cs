using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;

namespace IsoNet.Core.Transport;

public class TransportInvoker(AbstractTransport transport, ICodec<MethodCall> codec)
{
    private readonly MethodInvoker _invoker = new();

    public TransportInvoker Init()
    {
        transport.SetMessageHandler(msg => _invoker.Invoke(msg), codec);
        return this;
    }
    
    public void RegisterLocal<T>(T target)
    {
        _invoker.Register(target);
    }

    public T CreateRemote<T>() where T : class
    {
        var (remoteApi, _) = Proxy.Proxy.Create<T>(call => transport.SendMessage(call, codec));
        return remoteApi;
    }
}
