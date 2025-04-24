using System.Diagnostics;
using Common.TimeNS;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using IsoNet.Client.WebSocket;
using IsoNet.Core.IO.Codec;
using IsoNet.Iso.Client;
using IsoNet.Iso.Common.Json;
using IsoNet.Iso.Server;
using IsoNet.Server.WebSocket;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Iso;

public class IsoPlayerTests : AbstractTests
{
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
            remoteClientCreated.TrySetResult(client);
        };

        //
        // client
        var clientTransport = new WebSocketClient
        {
            Logger = CreateLogger("client")
        };
        var clientPlayer = new IsoPlayer();
        var clientCodec = IsoJsonCodecFactory.CreateCodec(clientPlayer).WrapLogging(clientTransport.Logger);
        var client = new IsoClient(clientPlayer, clientTransport, clientCodec).Init();
        await clientTransport.Connect("ws://localhost:7000/ws/");
        var remoteClient = await AwaitResult(remoteClientCreated);

        //
        // create cells
        const int width = 11;
        const int height = 12;
        var clientCellsCreated = CreateTaskCompletionSource(client.Player.Cells.Events, CellEvent.CellsCreated);
        var serverCellsCreated = CreateTaskCompletionSource(remoteClient.Player.Cells.Events, CellEvent.CellsCreated);
        client.RemoteApi.CreateCells(width, height);
        await AwaitResult(clientCellsCreated);
        await AwaitResult(serverCellsCreated);
        Assert.That(client.Player.Cells.Width, Is.EqualTo(width));
        Assert.That(client.Player.Cells.Heigth, Is.EqualTo(height));
        Assert.That(remoteClient.Player.Cells.Width, Is.EqualTo(width));
        Assert.That(remoteClient.Player.Cells.Heigth, Is.EqualTo(height));
        
        //
        // start
        var serverPlayerStarted = CreateTaskCompletionSource(remoteClient.Player);
        var clientPlayerStarted = CreateTaskCompletionSource(client.Player);
        client.RemoteApi.Start();
        await AwaitResult(serverPlayerStarted);
        await AwaitResult(clientPlayerStarted);
        
        //
        // build
        var serverBuildingCreated = CreateTaskCompletionSource(
            remoteClient.Player.Buildings.Events, BuildingEvent.BuildingCreated);
        var clientBuildingCreated = CreateTaskCompletionSource(
            client.Player.Buildings.Events, BuildingEvent.BuildingCreated);
        var buildingInfo = new BuildingInfo
        {
            Id = "b0",
            width = 2,
            height = 2
        };
        const int buildingX = 1;
        const int buildingY = 2;
        client.RemoteApi.Build(buildingInfo, client.Player.Cells.Get(buildingX, buildingY));
        var serverBuilding = await AwaitResult(serverBuildingCreated);
        Assert.That(serverBuilding.X, Is.EqualTo(buildingX));
        Assert.That(serverBuilding.Y, Is.EqualTo(buildingY));
        var clientBuilding = await AwaitResult(clientBuildingCreated);
        Assert.That(clientBuilding.X, Is.EqualTo(buildingX));
        Assert.That(clientBuilding.Y, Is.EqualTo(buildingY));
        
        //
        // dispose
        await clientTransport.Disconnect();
        serverTransport.Stop();
    }
}
