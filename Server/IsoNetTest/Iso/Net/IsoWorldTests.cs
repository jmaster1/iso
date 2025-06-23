using System.Diagnostics;
using Common.TimeNS;
using Iso.Player;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Iso.Net;

public class IsoWorldTests : AbstractIsoNetTests
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
        timer.Start(TimeSpan.FromMilliseconds(20), time);
        Thread.Sleep(10000);
        timer.Stop();
        stopwatch.Stop();
        Logger.LogInformation("Updates: {updates}, time: {time} ms", time.Frame, stopwatch.Elapsed.TotalMilliseconds.ToString("0.000"));
    }
    
    [Test]
    public async Task TestClientServer()
    {
        IsoTestContext.InitContext();
        
        var (isoServer, clientFactory) = 
            CreateServerWebsocket();
            //CreateServerLocal();

        var (client, remoteClient) = await CreateClient(isoServer, clientFactory);
        
        //
        // create world
        const int width = 11;
        const int height = 12;
        var serverWorld = await CreateWorld(isoServer, client, width, height);
        client.JoinWorld(serverWorld.Id);
        
        //
        // start
        var worldsSource = new MultiSource<IsoWorld>(serverWorld.World, client.World);
        await StartWorld(client, worldsSource);
        
        //
        // build
        await Build(client, worldsSource, IsoTestContext.BuildingId, 1, 1);

        Thread.Sleep(TimeSpan.FromSeconds(1));
        
        //
        // dispose
        await client.Disconnect();
        isoServer.Stop();
    }
}
