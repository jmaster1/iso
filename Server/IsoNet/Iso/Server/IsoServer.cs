using System.Collections.Concurrent;
using Iso.Cells;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server;
using IsoNet.Iso.Common;
using IsoNet.Iso.Common.Json;

namespace IsoNet.Iso.Server;

public class IsoServer(AbstractServer server)
{
    public event Action<IsoRemoteClient>? OnClientConnected;
    
    public event Action<ServerWorld>? OnWorldCreated;

    private readonly ConcurrentDictionary<string, ServerWorld> _worlds = new();
        
    public IsoServer Init()
    {
        server.OnClientConnected += InitTransport;
        return this;
    }

    private void InitTransport(AbstractTransport transport)
    {
        var codec = IsoJsonCodecFactory.CreateCodec().WrapLogging(transport.Logger);
        var client = new IsoRemoteClient(this, transport, codec).Init();
        OnClientConnected?.Invoke(client);
    }

    internal ServerWorld CreateWorld(int width, int height, IsoRemoteClient client)
    {
        var world = new IsoWorld(Guid.NewGuid().ToString());
        world.Cells.Create(width, height, () =>
        {
            world.Cells.ForEachPos((x, y) => world.Cells.Set(x, y, CellType.Buildable));    
        });
        var serverWorld = new ServerWorld(world);
        serverWorld.Clients.Add(client);
        _worlds[world.Id] = serverWorld;
        OnWorldCreated?.Invoke(serverWorld);


        var worldInfo = new WorldInfo
        {
            Id = world.Id,
            Width = width,
            Height = height
        };
        serverWorld.ForEachClient(cln => cln.ClientApi.WorldСreated(worldInfo));
        return serverWorld;
    }
}
