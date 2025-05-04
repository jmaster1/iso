using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;

namespace IsoNet.Core.Transport;

public class TransportInvoker(AbstractTransport transport, ICodec<MethodCall> codec) : MethodInvoker
{
    public TransportInvoker Init(Action<MethodCall> handler = null)
    {
        transport.SetMessageHandler(Invoke, codec);
        return this;
    }
    
    public void RegisterLocal<T>(T target)
    {
        Register(target);
    }

    public T CreateRemote<T>() where T : class
    {
        var (remoteApi, _) = Proxy.Proxy.Create<T>(call => transport.SendMessage(call, codec));
        return remoteApi;
    }
}
