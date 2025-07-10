namespace IsoNetTest.Iso.Net;

public class IsoServerRunTests : AbstractIsoNetTests
{
    
    [Test]
    public async Task TestServerStart()
    {
        IsoTestContext.InitContext();
        
        var (isoServer, clientFactory) = 
            CreateServerWebsocket();
        while (true)
        {
            Thread.Sleep(100);
        }
        isoServer.Stop();
    }
}
