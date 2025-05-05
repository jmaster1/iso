using System.Net.WebSockets;
using IsoNet.Core.IO;
using Microsoft.Extensions.Logging;

namespace IsoNet.Core.Transport.WebSocket;

public class WebSocketTransport : AbstractTransport
{
    internal System.Net.WebSockets.WebSocket? WebSocket;

    public int BufferSize = 1024;

    private BufferedCallbackOutputStream? _outputStream;

    protected override void DoWithOutput(Action<Stream> action)
    {
        if (!IsConnected())
        {
            throw new Exception("Not connected: " + WebSocket?.State);
        }

        if (_outputStream == null)
        {
            var buffer = new byte[BufferSize];
            _outputStream = new BufferedCallbackOutputStream(buffer, (_, count, eof) => 
                WebSocket!.SendAsync(new ArraySegment<byte>(buffer, 0, count), 
                    WebSocketMessageType.Binary, eof, CancellationToken.None));
        }

        _outputStream.Reset();
        action(_outputStream);
    }

    protected override async Task DisconnectInternal()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            await WebSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "disconnect", cts.Token)!;
        }
        finally
        {
            WebSocket?.Dispose();
            WebSocket = null;
            Closed();
        }
    }

    public override bool IsConnected()
    {
        return WebSocket is {State: WebSocketState.Open or WebSocketState.CloseReceived};
    }

    internal async Task ListenForMessages()
    {
        Logger?.LogInformation("ListenForMessages begin");
        try
        {
            var buffer = new byte[BufferSize];
            var arraySegment = new ArraySegment<byte>(buffer);
            
            async Task<WebSocketReceiveResult> ReceiveAsync()
            {
                var result = await WebSocket?.ReceiveAsync(arraySegment, CancellationToken.None)!;

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Logger?.LogInformation("Close message received");

                    if (IsConnected() || WebSocket?.State == WebSocketState.CloseReceived)
                    {
                        await Disconnect();
                    }
                }

                return result;
            }

            await using var stream = new BufferedCallbackInputStream(buffer, _ =>
            {
                var result = ReceiveAsync().GetAwaiter().GetResult();
                return result.MessageType == WebSocketMessageType.Close ? (0, true) : (result.Count, result.EndOfMessage);
            });
            
            while (IsConnected())
            {
                try
                {
                    var result = await ReceiveAsync();
                    if (result.MessageType == WebSocketMessageType.Close) continue;
                    stream.Filled(result.Count, result.EndOfMessage);
                    MessageReceived(stream);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Message processing error");
                    throw;
                }
            }
        }
        finally
        {
            Logger?.LogInformation("ListenForMessages end");
        }
    }
}
