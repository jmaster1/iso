using System.Diagnostics;
using Common.TimeNS;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport.Server.WebSocket;
using IsoNet.Core.Transport.WebSocket;
using IsoNet.Iso.Client;
using IsoNet.Iso.Common.Json;
using IsoNet.Iso.Server;
using IsoNetTest.Core;
using IsoNetTest.Core.Log;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Iso;

public class IsoWorldTests : AbstractTests
{
    protected override void ConfigureLoggingBuilder(ILoggingBuilder builder)
    {
        builder.AddProvider(TransportRmiHtmlLogger.Provider);
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

    [Test]
    public async Task TestClientServer()
    {
        //
        // server
        var serverTransport = new WebSocketServer("http://localhost:7000/ws/")
        {
            Logger = CreateLogger("server")
        };
        serverTransport.Start();
        var server = new IsoServer(serverTransport).Init();

        var remoteClientCreated = new TaskCompletionSource<IsoRemoteClient>();
        server.OnClientConnected += client =>
        {
            client.Rmi.Logger = CreateLogger("serverRmi");
            remoteClientCreated.TrySetResult(client);
        };

        //
        // client
        var clientTransport = new WebSocketClient
        {
            Logger = CreateLogger("client")
        };
        var clientPlayer = new IsoWorld();
        var clientCodec = IsoJsonCodecFactory.CreateCodec().WrapLogging(clientTransport.Logger);
        var client = new IsoClient(clientPlayer, clientTransport, clientCodec).Init();
        client.Rmi.Logger = CreateLogger("clientRmi");
        client.Rmi.RequestIdOffset = 1000;
        await clientTransport.Connect("ws://localhost:7000/ws/");
        var remoteClient = await AwaitResult(remoteClientCreated);
        
        //
        // create world
        const int width = 11;
        const int height = 12;
        var serverWorldCreated = CreateTaskCompletionSource<WorldPlayers>(tcs =>
        {
            server.OnWorldCreated += worldPlayers => tcs.TrySetResult(worldPlayers);
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
        await clientTransport.Disconnect();
        serverTransport.Stop();
    }
}
