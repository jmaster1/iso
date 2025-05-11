using IsoNet.Core.IO.Codec;
using Microsoft.Extensions.Logging;

namespace IsoNet.Core.Transport;

public abstract class AbstractTransport : LogAware
{
    public event Action? OnClose;
    
    /// <summary>
    /// this should be triggered by subclasses for each incoming message stream
    /// </summary>
    protected Action<Stream> MessageReceived = _ => {};
    
    public int MessageCountSent { get; private set; }
    
    public int MessageCountReceived { get; private set; }

    public bool LogMessagesIn;
    
    public bool LogMessagesOut;
    
    /// <summary>
    /// should be invoked by subclasses when transport is closed
    /// </summary>
    protected void Closed()
    {
        Logger?.LogInformation("Closed");
        OnClose?.Invoke();
    }

    public void SetMessageHandler<T>(Action<T> handler, ICodec<T> codec)
    {
        MessageReceived = stream =>
        {
            var msg = codec.Read(stream);
            MessageCountReceived++;
            if(LogMessagesIn) Logger?.LogInformation("<< {msg}", msg);
            handler(msg);
        };
    }

    public void SendMessage<T>(T msg, ICodec<T> codec)
    {
        SendMessage(stream =>
        {
            codec.Write(msg, stream);
        });
    }
    
    public void SetMessageHandler(Action<Stream> messageReader)
    {
        MessageReceived = stream =>
        {
            messageReader(stream);
            MessageCountReceived++;
        };
    }
    
    public void SendMessage(Action<Stream> messageWriter)
    {
        DoWithOutput(stream =>
        {
            try
            {
                messageWriter(stream);
                MessageCountSent++;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error sending message");
                throw;
            }
        });
    }

    /// <summary>
    /// subclasses should implement this method in order to initiate outgoing message output stream
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected abstract void DoWithOutput(Action<Stream> action);
    
    public async Task Disconnect()
    {
        Logger?.LogInformation("Disconnecting...");
        if (!IsConnected())
        {
            Logger?.LogInformation("Not connected.");
            return;
        }
        try
        {
            await DisconnectInternal();
        }
        catch (OperationCanceledException)
        {
            Logger?.LogInformation("Disconnect timed out, forcing close.");
        }
        catch (Exception ex)
        {
            Logger?.LogInformation("Error during disconnect: {ex}", ex);
        }
        finally
        {
            Closed();
        }
    }

    protected abstract Task DisconnectInternal();

    public abstract bool IsConnected();
}
