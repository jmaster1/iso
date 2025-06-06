using Common.Api.Info;
using Common.ContextNS;
using Common.TimeNS;
using Iso.Buildings;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server;
using IsoNet.Core.Transport.Server.WebSocket;
using IsoNet.Core.Transport.WebSocket;
using IsoNet.Iso.Client;
using IsoNet.Iso.Common;
using IsoNet.Iso.Common.Json;
using IsoNet.Iso.Server;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Iso.Net;

public abstract class AbstractIsoNetTests : AbstractTests
{
    protected override void ConfigureLoggingBuilder(ILoggingBuilder builder)
    {
        AddTransportRmiHtmlLogger(builder);
    }
    
    protected const string BuildingId = "b0";
    
    protected static void InitContext()
    {
        var infoApi = Context.Get<InfoApi>();
        infoApi.loaders.Add((_, type) =>
        {
            if (type == typeof(List<BuildingInfo>))
            {
                return new List<BuildingInfo> { 
                    new()
                    {
                        Id = BuildingId,
                        width = 2,
                        height = 2
                    }
                };
            }
            throw new NotImplementedException();
        });
    }

    private static IsoServer CreateServer(AbstractServer server)
    {
        server.Logger = CreateLogger("server");
        var isoServer = new IsoServer(server).Init();
        isoServer.OnClientConnected += client =>
        {
            client.Rmi.Logger = CreateLogger("serverRmi");
        };
        return isoServer;
    }

    private static int _requestIdOffset = 1000;
    
    private static IsoClient CreateClient(AbstractTransport clientTransport)
    {
        clientTransport.Logger = CreateLogger("client");
        var isoWorld = new IsoWorld();
        var clientCodec = IsoJsonCodecFactory.CreateCodec().WrapLogging(clientTransport.Logger);
        var time = new Time();
        new TimeTimer().Start(time, IsoCommon.Delta);
        var isoClient = new IsoClient(isoWorld, clientTransport, clientCodec, time).Init();
        isoClient.Rmi.Logger = CreateLogger("clientRmi");
        isoClient.Rmi.RequestIdOffset = _requestIdOffset;
        _requestIdOffset += 1000;
        return isoClient;
    }
    
    protected static (IsoServer, Func<IsoClient>) CreateServerWebsocket()
    {
        var server = new WebSocketServer("http://localhost:7000/ws/");
        server.Start();
        var isoServer = CreateServer(server);
        return (isoServer, () =>
        {
            var clientTransport = new WebSocketClient();
            clientTransport.Connect("ws://localhost:7000/ws/").Wait();
            return CreateClient(clientTransport);
        });
    }
    
    protected static (IsoServer, Func<IsoClient>) CreateServerLocal()
    {
        var server = new LocalServerTransport();
        var isoServer = CreateServer(server);
        return (isoServer, () => CreateClient(server.AddClient()));
    }
    
    protected static async Task<(IsoClient client, IsoRemoteClient remoteClient)> 
        CreateClient(IsoServer isoServer, Func<IsoClient> clientFactory)
    {
        var remoteClientCreated = new TaskCompletionSource<IsoRemoteClient>();
        isoServer.OnClientConnected += remoteClient =>
        {
            remoteClientCreated.TrySetResult(remoteClient);
        };
        var client = clientFactory();
        var remoteClient = await AwaitResult(remoteClientCreated);
        return (client, remoteClient);
    }
    
    protected static async Task<ServerWorld> CreateWorld(IsoServer isoServer, IsoClient client, int width, int height)
    {
        var serverWorldCreated = CreateTcsAction<ServerWorld>(tcs =>
        {
            isoServer.OnWorldCreated += worldPlayers => tcs.TrySetResult(worldPlayers);
        });
        var worldId = client.CreateWorld(width, height);
        var serverWorld = await AwaitResult(serverWorldCreated);
        Assert.That(serverWorld.World.Id, Is.EqualTo(worldId));
        return serverWorld;
    }
    
    protected static async Task StartWorld(IsoClient client, MultiSource<IsoWorld> worldsSource)
    {
        var worldsStarted = worldsSource.CreateTcs(CreateTcsBindable);
        client.StartWorld();
        await worldsStarted.AwaitResults();
    }
    
    protected async Task Build(IsoClient client, MultiSource<IsoWorld> worldsSource, 
        string buildingId, int buildingX, int buildingY)
    {
        var buildingCreated = worldsSource.CreateTcs(player => 
            CreateTcsEvents(player.Buildings.Events, BuildingEvent.BuildingCreated));
        client.RemoteWorldApi.Build(BuildingId, buildingX, buildingY);
        await buildingCreated.AwaitResults((_, building) =>
        {
            Assert.That(building!.X, Is.EqualTo(buildingX));
            Assert.That(building.Y, Is.EqualTo(buildingY));    
        });
    }
}
