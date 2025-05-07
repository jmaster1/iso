using System.Collections.Concurrent;
using Common.Lang.Observable;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server;
using IsoNet.Iso.Common.Json;

namespace IsoNet.Iso.Server;

public class IsoServer(AbstractServer server)
{
    public event Action<IsoRemoteClient>? OnClientConnected;
    
    public event Action<IsoPlayer>? OnWorldCreated;

    private ConcurrentDictionary<string, IsoPlayer> _worlds = new();
        
    public IsoServer Init()
    {
        server.OnClientConnected += InitTransport;
        return this;
    }

    private void InitTransport(AbstractTransport transport)
    {
        var player = new IsoPlayer(null!);
        var codec = IsoJsonCodecFactory.CreateCodec(player).WrapLogging(transport.Logger);
        var client = new IsoRemoteClient(this, transport, codec, player).Init();
        OnClientConnected?.Invoke(client);
    }

    internal IsoPlayer CreateWorld()
    {
        var world = new IsoPlayer(Guid.NewGuid().ToString());
        _worlds[world.Guid] = world;
        OnWorldCreated?.Invoke(world);
        return world;
    }
}
