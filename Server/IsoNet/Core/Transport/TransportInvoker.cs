using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;

namespace IsoNet.Core.Transport;

public class TransportInvoker(
    AbstractTransport transport, 
    ICodec codec
    ) : MethodInvoker
{
    public TransportInvoker Init(Action<MethodCall>? handler = null)
    {
        //transport.SetMessageHandler(handler ?? InvokeNoResult, codec);
        return this;
    }

    private void InvokeNoResult(MethodCall obj)
    {
        Invoke(obj);
    }

    public void RegisterLocal<T>(T target)
    {
        Register(target);
    }

    public T CreateRemote<T>(Action<MethodCall>? filter = null) where T : class
    {
        var (remoteApi, _) = Proxy.Proxy.Create<T>(call =>
        {
            filter?.Invoke(call);
            //transport.SendMessage(call, codec);
            return null;
        });
        return remoteApi;
    }
}
