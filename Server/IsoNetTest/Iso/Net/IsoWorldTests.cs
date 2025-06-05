using System.Diagnostics;
using Common.Api.Info;
using Common.ContextNS;
using Common.TimeNS;
using Iso.Buildings;
using Iso.Player;
using IsoNet.Iso.Server;
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
        timer.Start(time, TimeSpan.FromMilliseconds(20));
        Thread.Sleep(10000);
        timer.Stop();
        stopwatch.Stop();
        Logger.LogInformation("Updates: {updates}, time: {time} ms", time.Frame, stopwatch.Elapsed.TotalMilliseconds.ToString("0.000"));
    }
    
    const string BuildingId = "b0";
    
    private static void InitContext()
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
    
    [Test]
    public async Task TestClientServer()
    {
        InitContext();
        
        var (isoServer, clientFactory) = 
            CreateServerWebsocket();
            //CreateServerLocal();

        var remoteClientCreated = new TaskCompletionSource<IsoRemoteClient>();
        isoServer.OnClientConnected += remoteClient =>
        {
            remoteClientCreated.TrySetResult(remoteClient);
        };

        var client = clientFactory();
        var remoteClient = await AwaitResult(remoteClientCreated);
        
        //
        // create world
        const int width = 11;
        const int height = 12;
        var serverWorldCreated = CreateTaskCompletionSource<ServerWorld>(tcs =>
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
        const int buildingX = 1;
        const int buildingY = 2;
        client.RemoteWorldApi.Build(BuildingId, buildingX, buildingY);
        await buildingCreated.AwaitResults((_, building) =>
        {
            Assert.That(building!.X, Is.EqualTo(buildingX));
            Assert.That(building.Y, Is.EqualTo(buildingY));    
        });

        //
        // dispose
        // await client.Start().Disconnect();
        // server.Stop();
    }
}
