using System.Diagnostics;
using Common.TimeNS;
using Iso.Buildings;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server;
using IsoNet.Core.Transport.Server.WebSocket;
using IsoNet.Core.Transport.WebSocket;
using IsoNet.Iso.Client;
using IsoNet.Iso.Common.Json;
using IsoNet.Iso.Server;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Iso;

public class IsoWorldTests : AbstractTests
{
    protected override void ConfigureLoggingBuilder(ILoggingBuilder builder)
    {
        AddTransportRmiHtmlLogger(builder);
    }

    [Test]
    public void Test()
    {
        var stopwatch = Stopwatch.StartNew();
        //var player = new IsoPlayer();
        var time = new Time();
        DateTime? lastUpdate = null;
        time.AddListener(_ =>
        {
            var dt = lastUpdate != null ? DateTime.Now - lastUpdate : TimeSpan.Zero;
            Logger.LogInformation("Update: {n}, dt = {dt}", time.Frame, dt.Value.TotalMilliseconds);
            lastUpdate = DateTime.Now;
        });
        //player.Bind(time);
        var timer = new TimeTimer();
        timer.Start(time, TimeSpan.FromMilliseconds(20));
        Thread.Sleep(10000);
        timer.Stop();
        stopwatch.Stop();
        Logger.LogInformation("Updates: {updates}, time: {time} ms", time.Frame, stopwatch.Elapsed.TotalMilliseconds.ToString("0.000"));
    }

    private (IsoServer, IsoClient, Action) CreateClientServer(
        AbstractServer server, AbstractTransport clientTransport, Action starter)
    {
        server.Logger = CreateLogger("server");
        clientTransport.Logger = CreateLogger("client");
        
        var isoServer = new IsoServer(server).Init();
        isoServer.OnClientConnected += client =>
        {
            client.Rmi.Logger = CreateLogger("serverRmi");
        };
        
        var isoWorld = new IsoWorld();
        var clientCodec = IsoJsonCodecFactory.CreateCodec().WrapLogging(clientTransport.Logger);
        var isoClient = new IsoClient(isoWorld, clientTransport, clientCodec).Init();
        isoClient.Rmi.Logger = CreateLogger("clientRmi");
        isoClient.Rmi.RequestIdOffset = 1000;
        
        return (isoServer, isoClient, starter);
    }
    
    private (IsoServer, IsoClient, Action) CreateClientServerWebsocket()
    {
        var server = new WebSocketServer("http://localhost:7000/ws/")
        {
            Logger = CreateLogger("server")
        };
        
        var clientTransport = new WebSocketClient
        {
            Logger = CreateLogger("client")
        };
        
        return CreateClientServer(server, clientTransport, () =>
        {
            server.Start();
            clientTransport.Connect("ws://localhost:7000/ws/");
        });
    }

    private (IsoServer, IsoClient, Action) CreateClientServerLocal()
    {
        var (transportCln, transportSrv) = LocalTransport.CreatePair();
        var server = LocalTransport.CreateServer(transportSrv);
        return CreateClientServer(server, transportCln, () => server.Start());
    }

    [Test]
    public async Task TestClientServer()
    { 
        var (isoServer, client, start) = 
            CreateClientServerLocal();
            //CreateClientServerWebsocket();

        var remoteClientCreated = new TaskCompletionSource<IsoRemoteClient>();
        isoServer.OnClientConnected += client =>
        {
            remoteClientCreated.TrySetResult(client);
        };
        start();
        var remoteClient = await AwaitResult(remoteClientCreated);
        
        //
        // create world
        const int width = 11;
        const int height = 12;
        var serverWorldCreated = CreateTaskCompletionSource<WorldPlayers>(tcs =>
        {
            isoServer.OnWorldCreated += worldPlayers => tcs.TrySetResult(worldPlayers);
        });
        var clientWorldCreated = CreateTaskCompletionSource(client.WorldId);
        client.CreateWorld(width, height);
        var serverWorldPlayers = await AwaitResult(serverWorldCreated);
        var clientWorldId = await AwaitResult(clientWorldCreated);
        Assert.That(serverWorldPlayers.World.Id, Is.EqualTo(clientWorldId));
        
        //
        // start
        var cs2 = new MultiSource<IsoWorld>(client.World, remoteClient.World);
        var playerStarted = cs2.CreateTaskCompletionSource(CreateTaskCompletionSource);
        client.Start();
        await playerStarted.AwaitResults();
        
        //
        // build
        var buildingCreated = cs2.CreateTaskCompletionSource(player => 
            CreateTaskCompletionSource(player.Buildings.Events, BuildingEvent.BuildingCreated));
        var buildingInfo = new BuildingInfo
        {
            Id = "b0",
            width = 2,
            height = 2
        };
        const int buildingX = 1;
        const int buildingY = 2;
        client.RemoteWorldApi.Build(buildingInfo, client.World.Cells.Get(buildingX, buildingY));
        await buildingCreated.AwaitResults((_, building) =>
        {
            Assert.That(building.X, Is.EqualTo(buildingX));
            Assert.That(building.Y, Is.EqualTo(buildingY));    
        });

        //
        // dispose
        // await clientTransport.Disconnect();
        // serverTransport.Stop();
    }
}
