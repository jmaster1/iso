using System.Net;
using System.Net.WebSockets;
using IsoNet.Core.Transport.WebSocket;
using Microsoft.Extensions.Logging;

namespace IsoNet.Server.WebSocket;

public class WebSocketServer : AbstractServer
{
    private readonly HttpListener _listener = new();

    public WebSocketServer(string url)
    {
        _listener.Prefixes.Add(url);
    }

    protected override void StartInternal()
    {
        _listener.Start();
        Task.Run(async () =>
        {
            Logger?.LogInformation("server loop enter");
            try 
            {
                while (IsRunning())
                {
                    try
                    {
                        var context = await _listener.GetContextAsync();
                        if (!context.Request.IsWebSocketRequest)
                        {
                            context.Response.StatusCode = 400;
                            context.Response.Close();
                            continue;
                        }

                        WebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
                        var transport = new WebSocketTransport
                        {
                            WebSocket = wsContext.WebSocket,
                            Logger = Logger
                        };
                        ClientConnected(transport);
                        _ = transport.ListenForMessages();
                    }
                    catch (Exception ex)
                    {
                        if (IsRunning()) Logger?.LogInformation($"Error: {ex.Message}");
                    }
                }
            }
            finally
            {
                Logger?.LogInformation("server loop exit");
            }
        });
    }
    

    protected override void StopInternal()
    {
        _listener.Stop();
    }

    public override bool IsRunning()
    {
        return _listener.IsListening;
    }
}