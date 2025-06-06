using System.Collections.Concurrent;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server;
using IsoNet.Iso.Common;
using IsoNet.Iso.Common.Json;

namespace IsoNet.Iso.Server;

public class IsoServer(AbstractServer server)
{
    public AbstractServer Server => server;
    public event Action<IsoRemoteClient>? OnClientConnected;
    
    public event Action<ServerWorld>? OnWorldCreated;

    public readonly ConcurrentDictionary<string, ServerWorld> Worlds = new();
        
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

    internal ServerWorld CreateWorld(int width, int height)
    {
        var world = new IsoWorld(Guid.NewGuid().ToString());
        IsoCommon.InitWorld(world, width, height);
        var serverWorld = new ServerWorld(world);
        Worlds[world.Id] = serverWorld;
        OnWorldCreated?.Invoke(serverWorld);
        return serverWorld;
    }

    public void Stop()
    {
        server.Stop();
    }

    public ServerWorld GetWorld(string worldId)
    {
        return Worlds[worldId];
    }
}
