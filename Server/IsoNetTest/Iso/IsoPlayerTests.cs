using System.Diagnostics;
using Common.TimeNS;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using IsoNet.Client.WebSocket;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Iso.Client;
using IsoNet.Iso.Common.Json;
using IsoNet.Iso.Server;
using IsoNet.Server.WebSocket;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;

namespace IsoNetTest;

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
        time.StartTimer(TimeSpan.FromMilliseconds(20));
        Thread.Sleep(10000);
        time.StopTimer();
        stopwatch.Stop();
        Logger.LogInformation("Updates: {updates}, time: {time} ms", time.Frame, stopwatch.Elapsed.TotalMilliseconds.ToString("0.000"));
    }

    [Test]
    public async Task TestClientServer()
    {
        //var codec = MethodCallJsonConverter.Codec;

        //
        // server
        var serverTransport = new WebSocketServer("http://localhost:7000/ws/")
        {
            Logger = CreateLogger("server")
        };
        serverTransport.Start();
        var server = new IsoServer(serverTransport).Init();

        var remoteClientTcs = new TaskCompletionSource<IsoRemoteClient>();
        server.OnClientConnected += client =>
        {
            remoteClientTcs.TrySetResult(client);
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
        
        var remoteClient = await AwaitResult(remoteClientTcs);
        
        //
        // create cells
        const int width = 11;
        const int height = 12;
        var cellsCreatedTcs = CreateTaskCompletionSource(
            client.Player.Cells.Events, CellEvent.CellsCreated);
        client.RemoteApi.CreateCells(width, height);
        await AwaitResult(cellsCreatedTcs);
        Assert.That(client.Player.Cells.Width, Is.EqualTo(width));
        Assert.That(client.Player.Cells.Heigth, Is.EqualTo(height));
        Assert.That(remoteClient.Player.Cells.Width, Is.EqualTo(width));
        Assert.That(remoteClient.Player.Cells.Heigth, Is.EqualTo(height));
        
        //
        // start
        client.RemoteApi.Start();
        Thread.Sleep(555);
        
        //
        // build
        var buildingCreatedTcs = CreateTaskCompletionSource(
            client.Player.Buildings.Events, BuildingEvent.BuildingRemoved);
        var buildingInfo = new BuildingInfo
        {
            Id = "b0",
            width = 2,
            height = 2
        };
        client.RemoteApi.Build(buildingInfo, client.Player.Cells.Get(0, 0));
        await AwaitResult(buildingCreatedTcs);
        
        //
        // dispose
        await clientTransport.Disconnect();
        serverTransport.Stop();
    }


}

