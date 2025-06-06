using IsoNet.Core.Log;
using IsoNet.Core.Log.Appender;
using IsoNet.Core.Transport.Server.WebSocket;
using IsoNet.Iso.Server;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    FileAppender.AnnounceAppender = ConsoleAppender.Instance;
    builder
        .SetMinimumLevel(LogLevel.Debug)
        .AddProvider(AbstractLogger.LoggerProvider<DefaultLogger>(
            ConsoleAppender.Instance));
});

var server = new WebSocketServer("http://localhost:7000/ws/");
server.Logger = loggerFactory.CreateLogger("server");
server.Start();

var isoServer = new IsoServer(server).Init();
isoServer.OnClientConnected += client =>
{
    client.Rmi.Logger = loggerFactory.CreateLogger("serverRmi");
};

await Task.Delay(-1);
