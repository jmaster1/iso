using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Server;

namespace IsoNet.Iso.Server;

public class IsoServer(AbstractServer server, ICodec<MethodCall> codec)
{
    public event Action<IsoRemoteClient>? OnClientConnected;
    
    public IsoServer Init()
    {
        server.OnClientConnected += InitTransport;
        return this;
    }

    private void InitTransport(AbstractTransport transport)
    {
        var client = new IsoRemoteClient(transport, codec, new IsoPlayer()).Init();
        OnClientConnected?.Invoke(client);
    }
}
