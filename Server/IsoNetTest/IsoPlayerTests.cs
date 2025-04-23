using System.Diagnostics;
using Common.TimeNS;
using Common.Util.Reflect;
using Iso.Cells;
using IsoNet.Client.WebSocket;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Iso.Client;
using IsoNet.Iso.Server;
using IsoNet.Server.WebSocket;
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
        var codec = MethodCallJsonConverter.Codec;

        //
        // server
        var serverTransport = new WebSocketServer("http://localhost:7000/ws/")
        {
            Logger = CreateLogger("server")
        };
        serverTransport.Start();
        var server = new IsoServer(serverTransport, codec.WrapLogging(serverTransport.Logger)).Init();

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
        var client = new IsoClient(clientTransport, codec.WrapLogging(clientTransport.Logger)).Init();
        await clientTransport.Connect("ws://localhost:7000/ws/");
        
        var remoteClient = await AwaitResult(remoteClientTcs);
        
        //
        // create cells
        const int width = 11;
        const int height = 12;
        var cellsTcs = new TaskCompletionSource<CellEvent>();
        client.Player.Cells.Events.AddListener((cellEvent, _) =>
        {
            switch (cellEvent)
            {
                case CellEvent.cellsCreated:
                    cellsTcs.TrySetResult(cellEvent);
                    break;
            }
        });
        client.RemoteApi.CreateCells(width, height);
        await AwaitResult(cellsTcs);
        Assert.That(client.Player.Cells.Width, Is.EqualTo(width));
        Assert.That(client.Player.Cells.Heigth, Is.EqualTo(height));
        Assert.That(remoteClient.Player.Cells.Width, Is.EqualTo(width));
        Assert.That(remoteClient.Player.Cells.Heigth, Is.EqualTo(height));
        
        //
        // dispose
        await clientTransport.Disconnect();
        serverTransport.Stop();
    }
}

