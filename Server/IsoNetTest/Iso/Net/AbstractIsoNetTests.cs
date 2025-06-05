using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server;
using IsoNet.Core.Transport.Server.WebSocket;
using IsoNet.Core.Transport.WebSocket;
using IsoNet.Iso.Client;
using IsoNet.Iso.Common.Json;
using IsoNet.Iso.Server;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Iso.Net;

public abstract class AbstractIsoNetTests : AbstractTests
{
    protected override void ConfigureLoggingBuilder(ILoggingBuilder builder)
    {
        AddTransportRmiHtmlLogger(builder);
    }

    protected IsoServer CreateServer(AbstractServer server)
    {
        server.Logger = CreateLogger("server");
        var isoServer = new IsoServer(server).Init();
        isoServer.OnClientConnected += client =>
        {
            client.Rmi.Logger = CreateLogger("serverRmi");
        };
        return isoServer;
    }
    
    protected IsoClient CreateClient(AbstractTransport clientTransport)
    {
        clientTransport.Logger = CreateLogger("client");
        var isoWorld = new IsoWorld();
        var clientCodec = IsoJsonCodecFactory.CreateCodec().WrapLogging(clientTransport.Logger);
        var isoClient = new IsoClient(isoWorld, clientTransport, clientCodec).Init();
        isoClient.Rmi.Logger = CreateLogger("clientRmi");
        isoClient.Rmi.RequestIdOffset = 1000;
        return isoClient;
    }
    
    protected (IsoServer, Func<IsoClient>) CreateServerWebsocket()
    {
        var server = new WebSocketServer("http://localhost:7000/ws/");
        server.Start();
        var isoServer = CreateServer(server);
        return (isoServer, () =>
        {
            var clientTransport = new WebSocketClient();
            clientTransport.Connect("ws://localhost:7000/ws/").Wait();
            return CreateClient(clientTransport);
        });
    }
    
    protected (IsoServer, Func<IsoClient>) CreateServerLocal()
    {
        var server = new LocalServerTransport();
        var isoServer = CreateServer(server);
        return (isoServer, () => CreateClient(server.AddClient()));
    }
}
