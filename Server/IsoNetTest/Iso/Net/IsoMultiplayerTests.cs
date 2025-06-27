using Iso.Player;

namespace IsoNetTest.Iso.Net;

public class IsoMultiplayerTests : AbstractIsoNetTests
{
    
    [Test]
    public async Task TestClientServer()
    {
        IsoTestContext.InitContext();
        
        var (isoServer, clientFactory) = 
            //CreateServerWebsocket();
            CreateServerLocal();

        var (clientA, remoteClientA) = await CreateClient(isoServer, clientFactory, "clnA");
        var (clientB, remoteClientB) = await CreateClient(isoServer, clientFactory, "clnB");

        var serverWorld = await CreateWorld(isoServer, clientA, 20, 20);
        clientA.JoinWorld(serverWorld.Id);
        clientB.JoinWorld(serverWorld.Id);
        
        var worldsSource = new MultiSource<IsoWorld>(serverWorld.World, 
            clientA.World, clientB.World);
        await StartWorld(clientA, worldsSource);
        
        await Build(clientA, worldsSource, IsoTestContext.BuildingId, 1, 1);
        
        //
        // dispose
        await clientA.Disconnect();
        await clientB.Disconnect();
        isoServer.Stop();
    }
}
