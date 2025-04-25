using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server;
using IsoNet.Iso.Common.Json;

namespace IsoNet.Iso.Server;

public class IsoServer(AbstractServer server)
{
    public event Action<IsoRemoteClient>? OnClientConnected;
    
    public IsoServer Init()
    {
        server.OnClientConnected += InitTransport;
        return this;
    }

    private void InitTransport(AbstractTransport transport)
    {
        var player = new IsoPlayer();
        var codec = IsoJsonCodecFactory.CreateCodec(player).WrapLogging(transport.Logger);
        var client = new IsoRemoteClient(transport, codec, player).Init();
        OnClientConnected?.Invoke(client);
    }
}
