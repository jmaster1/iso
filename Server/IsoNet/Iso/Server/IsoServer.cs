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
    
    public event Action<WorldPlayers>? OnWorldCreated;

    private ConcurrentDictionary<string, WorldPlayers> _worlds = new();
        
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

    internal WorldPlayers CreateWorld(int width, int height, IsoRemoteClient client)
    {
        var world = new IsoWorld(Guid.NewGuid().ToString());
        world.Cells.Create(width, height, () =>
        {
            world.Cells.ForEachPos((x, y) => world.Cells.Set(x, y, CellType.Buildable));    
        });
        var worldPlayers = new WorldPlayers()
        {
            World = world,
        };
        worldPlayers.Clients.Add(client);
        _worlds[world.Id] = worldPlayers;
        OnWorldCreated?.Invoke(worldPlayers);


        var worldInfo = new WorldInfo() 
        {
            Id = world.Id,
            Width = width,
            Height = height
        };
        worldPlayers.ForEachClient(cln => cln._clientApi.WorldСreated(worldInfo));
        return worldPlayers;
    }

    public void StartWorld(WorldPlayers worldPlayers, IsoRemoteClient client)
    {
        worldPlayers.Start();
        worldPlayers.ForEachClient(cln => cln.WorldStarted());
    }
}
