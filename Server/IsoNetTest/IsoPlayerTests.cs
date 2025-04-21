using System.Diagnostics;
using Common.TimeNS;
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
    public void TestClientServer()
    {
        var codec = MethodCallJsonConverter.Codec.WrapLogging(Logger);
        
        var srv = new WebSocketServer("http://localhost:7000/ws/");
        srv.Start();
        var server = new IsoServer(srv, codec);

        var clientTransport = new WebSocketClient();
        var client = new IsoClient(clientTransport);
    }
}

