using System.Net.WebSockets;
using Microsoft.Extensions.Logging;

namespace IsoNet.Core.Transport.WebSocket;

public class WebSocketClient : WebSocketTransport
{
    private CancellationTokenSource _cts = null!;

    public async Task Connect(string serverUrl)
    {
        if (WebSocket is {State: WebSocketState.Open})
        {
            Logger?.LogInformation("Already connected.");
            return;
        }

        var serverUri = new Uri(serverUrl);
        var cws = new ClientWebSocket();
        WebSocket = cws;
        _cts = new CancellationTokenSource();

        try
        {
            await cws.ConnectAsync(serverUri, _cts.Token);
            Logger?.LogInformation("Connected to {serverUri}", serverUri);
            _ = ListenForMessages();
        }
        catch (Exception ex)
        {
            Logger?.LogInformation("Error connecting to server: {error}", ex.Message);
        }
    }
}
