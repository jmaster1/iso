using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server.WebSocket;
using IsoNet.Core.Transport.WebSocket;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Common;

public class WebSocketTests : AbstractTests
{
    private static StringCodec Codec => StringCodec.Instance;
    
    [Test]
    public async Task TestWebSocketClientServer()
    {
        var codec = new StringCodec();
        
        var server = new WebSocketServer("http://localhost:7000/ws/")
        {
            Logger = CreateLogger("server")
        };
        AbstractTransport? serverTransport = null;
        server.OnClientConnected += transport =>
        {
            serverTransport = transport;
            transport.SetMessageHandler(stream =>
            {
                var msg = Codec.Read<string>(stream);
                server.Logger.LogInformation("handle " + msg);
            });
        };
        
        server.Start();
        Assert.That(server.CountActive, Is.EqualTo(0));
        
        var client = new WebSocketClient
        {
            Logger = CreateLogger("client")
        };
        
        // client.SetMessageHandler(msg =>
        // {
        //     client.Logger.LogInformation("handle " + msg);
        // }, codec);

        await client.Connect("ws://localhost:7000/ws/");
        Assert.That(server.CountActive, Is.EqualTo(1));

        const int messageCount = 11;
        for (var i = 0; i < messageCount; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(0.01));
            client.SendMessage(stream => codec.Write("Hello World " + i, stream));    
        }
        
        Thread.Sleep(TimeSpan.FromSeconds(1));
        
        Assert.That(client.MessageCountSent, Is.EqualTo(messageCount));
        Assert.That(serverTransport!.MessageCountReceived, Is.EqualTo(messageCount));
        

        await client.Disconnect();
        Thread.Sleep(TimeSpan.FromSeconds(1));
        Assert.That(server.CountActive, Is.EqualTo(0));
        Assert.That(server.ConnectCount, Is.EqualTo(1));
        Assert.That(server.DisconnectCount, Is.EqualTo(1));
        
        server.Stop();
    }
}
