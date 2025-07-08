using Common.TimeNS;
using Iso.Buildings;
using Iso.Player;
using Common.IO.Codec;
using Common.IO.Transport;
using Common.IO.Transport.Rmi;
using Common.IO.Transport.Server;
using Common.IO.Transport.Server.WebSocket;
using Common.IO.Transport.WebSocket;
using Iso.Net.Client;
using Iso.Net.Common;
using Iso.Net.Common.Json;
using IsoNet.Iso.Server;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Iso.Net;

public abstract class AbstractIsoNetTests : AbstractTests
{
    protected static IntSequence RequestIdSequence = new();
    
    protected override void ConfigureLoggingBuilder(ILoggingBuilder builder)
    {
        AddTransportRmiHtmlLogger(builder);
    }

    private static IsoServer CreateServer(AbstractServer server)
    {
        server.Logger = CreateLogger("server");
        var isoServer = new IsoServer(server).Init();
        isoServer.OnClientConnected += client =>
        {
            client.Rmi.Logger = CreateLogger("serverRmi");
            client.Rmi.RequestIdSeq = RequestIdSequence.NextVal;
        };
        return isoServer;
    }
    
    private static IsoClient CreateClient(AbstractTransport clientTransport)
    {
        clientTransport.Logger = CreateLogger("client");
        var isoWorld = new IsoWorld();
        var clientCodec = IsoJsonCodecFactory.CreateCodec().WrapLogging(clientTransport.Logger);
        var time = new Time();
        new TimerExecutor().Start(IsoCommon.Delta, time);
        var isoClient = new IsoClient(isoWorld, clientTransport, clientCodec, time).Init();
        isoClient.Rmi.Logger = CreateLogger("clientRmi");
        isoClient.Rmi.RequestIdSeq = RequestIdSequence.NextVal;
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
        CreateClient(
            IsoServer isoServer, 
            Func<IsoClient> clientFactory, 
            string? id = null)
    {
        var remoteClientCreated = new TaskCompletionSource<IsoRemoteClient>();
        isoServer.OnClientConnected += remoteClient =>
        {
            remoteClientCreated.TrySetResult(remoteClient);
        };
        var client = clientFactory();
        client.Id = id;
        var remoteClient = await AwaitResult(remoteClientCreated);
        remoteClient.Id = id;
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
        client.RemoteWorldApi.Build(IsoTestContext.BuildingId, buildingX, buildingY);
        await buildingCreated.AwaitResults((_, building) =>
        {
            Assert.That(building!.X, Is.EqualTo(buildingX));
            Assert.That(building.Y, Is.EqualTo(buildingY));    
        });
    }
}
