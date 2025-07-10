using Common.ContextNS;
using Common.Util.Http;
using Iso.Net.Client;
using Iso.Net.Common;

namespace IsoNetTest.Iso.Net;

public class IsoServerRunTests : AbstractIsoNetTests
{
    
    [Test]
    public async Task TestServerStart()
    {
        IsoTestContext.InitContext();
        
        var (isoServer, clientFactory) = 
            CreateServerWebsocket();

        // var client = clientFactory();
        // HttpDebug hd = new();
        // hd.Bind(Context.GetCurrent());
        // hd.Router.AddHandler(new TargetHttpQueryProcessor<IIsoServerApi>(client.ServerApi));
        // hd.Router.AddHandler(new TargetHttpQueryProcessor<IsoClient>(client));
        
        while (true)
        {
            Thread.Sleep(100);
        }
        isoServer.Stop();
    }
}

