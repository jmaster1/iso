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
    
    public event Action<IsoWorld>? OnWorldCreated;

    private ConcurrentDictionary<string, IsoWorld> _worlds = new();
        
    public IsoServer Init()
    {
        server.OnClientConnected += InitTransport;
        return this;
    }

    private void InitTransport(AbstractTransport transport)
    {
        var player = new IsoWorld(null!);
        var codec = IsoJsonCodecFactory.CreateCodec(player).WrapLogging(transport.Logger);
        var client = new IsoRemoteClient(this, transport, codec, player).Init();
        OnClientConnected?.Invoke(client);
    }

    internal IsoWorld CreateWorld(int width, int height)
    {
        var world = new IsoWorld(Guid.NewGuid().ToString());
        world.Cells.Create(width, height);
        _worlds[world.Id] = world;
        OnWorldCreated?.Invoke(world);
        return world;
    }
}
