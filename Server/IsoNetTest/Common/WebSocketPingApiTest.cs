using System.Diagnostics;
using IsoNet.Core;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Server.WebSocket;
using IsoNet.Core.Transport.WebSocket;
using IsoNetTest.Core;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Common;

internal interface IPingApi
{
    void Ping(string message);
    
    void Pong(string message);
}

internal class PingApi(IPingApi target) : LogAware, IPingApi
{
    public TaskCompletionSource<string> PongReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void Ping(string message)
    {
        Logger?.LogInformation("Ping({message})", message);
        target.Pong(message);
    }

    public void Pong(string message)
    {
        Logger?.LogInformation("Pong({message})", message);
        PongReceived.TrySetResult(message);
    }

    public void ResetPongReceived()
    {
        PongReceived = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}

public class WebSocketPingApiTest : AbstractTests
{
    [Test]
    public async Task Test()
    {
        //
        // server
        Func<string, Task>? pingFromServer = null;
        var server = new WebSocketServer("http://localhost:17011/ws/")
        {
            Logger = CreateLogger("server")
        };
        server.OnClientConnected += transport =>
        {
            var (serverRemoteApi, serverLocalApi) = InitTransport(transport);
            pingFromServer = async message =>
            {
                await Ping(transport, serverRemoteApi, serverLocalApi, message);
            };
        };
        server.Start();
        
        //
        // client
        var client = new WebSocketClient
        {
            Logger = CreateLogger("client")
        };
        var (clientRemoteApi, clientLocalApi) = InitTransport(client);
        await client.Connect("ws://localhost:17011/ws/");

        //
        // test
        for (var i = 0; i < 10; i++)
        {
            await Ping(client, clientRemoteApi, clientLocalApi, "ping_by_client_" + i);
            await pingFromServer!("ping_by_server_" + i);
        }

        await client.Disconnect();
        server.Stop();
    }

    private static async Task Ping(AbstractTransport transport, IPingApi remoteApi, PingApi localApi, string message)
    {
        localApi.ResetPongReceived();
        var stopwatch = Stopwatch.StartNew();
        remoteApi.Ping(message);
        var pong = await localApi.PongReceived.Task;
        stopwatch.Stop();
        transport.Logger!.LogInformation("Ping time: {time} ms", stopwatch.Elapsed.TotalMilliseconds.ToString("0.000"));
        Assert.That(pong, Is.EqualTo(message));
    }

    private static (IPingApi, PingApi) InitTransport(AbstractTransport transport)
    {
        var codec = new JsonCodec();//MethodCallJsonConverter.Codec.WrapLogging(transport.Logger);
        
        //
        // setup remote api proxy this will send message
        var (remoteApi, _) = Proxy.Create<IPingApi>(call =>
        {
            //transport.SendMessage(call, codec);
            return null!;
        });
        
        //
        // handle incoming messages
        var localApi = new PingApi(remoteApi)
        {
            Logger = transport.Logger
        };
        var invoker = new MethodInvoker();
        invoker.Register<IPingApi>(localApi);
        // transport.SetMessageHandler(msg => invoker.Invoke(msg), codec);
        // return (remoteApi, localApi);
        return (null, null);
    }
}
