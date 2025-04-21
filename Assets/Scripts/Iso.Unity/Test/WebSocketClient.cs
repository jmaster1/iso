using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Iso.Unity.Test
{
    public class WebSocketClient
    {
        public Action<string> _log;
        private ClientWebSocket? _webSocket;
        private CancellationTokenSource _cts;
        private Action<string> _onMessageReceived;
        private bool x;

        public async Task Connect(string serverUrl, Action<string> onMessageReceived)
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                _log("Already connected.");
                return;
            }

            var serverUri = new Uri(serverUrl);
            _webSocket = new ClientWebSocket();
            _cts = new CancellationTokenSource();
            _onMessageReceived = onMessageReceived;

            try
            {
                await _webSocket.ConnectAsync(serverUri, _cts.Token);
                _log($"Connected to {serverUri}");

                _ = ReceiveMessagesAsync(); // Запускаем фоновую обработку сообщений
            }
            catch (Exception ex)
            {
                _log($"Error connecting to server: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            _log("Disconnecting...");
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                _log("Not connected.");
                return;
            }

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", cts.Token);
            }
            catch (OperationCanceledException)
            {
                _log("Disconnect timed out, forcing close.");
            }
            catch (Exception ex)
            {
                _log($"Error during disconnect: {ex.Message}");
            }
            finally
            {
                _webSocket.Dispose();
                _webSocket = null;
                _log("Disconnected from server.");
            }
        }

        public async Task SendMessage(string message)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                _log("Not connected. Message not sent.");
                return;
            }

            var encodedMessage = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(encodedMessage), WebSocketMessageType.Text, true, _cts.Token);
            _log($">> {message}");
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024];

            while (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _log("Server closed connection.");
                        await Disconnect();
                    }
                    else
                    {
                        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        _log("<< " + receivedMessage);
                        _onMessageReceived?.Invoke(receivedMessage);
                    }
                }
                catch (Exception ex)
                {
                    if (_webSocket != null)
                    {
                        _log($"Error receiving message: {ex.Message}");
                    }
                    break;
                }
            }
        }
    }
}
