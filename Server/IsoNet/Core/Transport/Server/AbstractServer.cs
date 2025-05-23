using Microsoft.Extensions.Logging;

namespace IsoNet.Core.Transport.Server;

public abstract class AbstractServer : LogAware
{
    public event Action<AbstractTransport>? OnClientConnected;
    
    public int ConnectCount { get; private set; }
    
    public int DisconnectCount { get; private set; }

    public int CountActive => ConnectCount - DisconnectCount;
    
    protected void ClientConnected(AbstractTransport transport)
    {
        ConnectCount++;
        Logger?.LogInformation("client connected {transport}", transport);
        OnClientConnected?.Invoke(transport);
        transport.OnClose += () =>
        {
             DisconnectCount++;
        };
    }

    public void Start()
    {
        if (IsRunning()) return;
        Logger?.LogInformation("Starting...");
        StartInternal();
        Logger?.LogInformation("Started");
    }

    protected abstract void StartInternal();

    public void Stop()
    {
        if (!IsRunning()) return;
        Logger?.LogInformation("Stopping...");
        StopInternal();
        Logger?.LogInformation("Stopped");
    }

    protected abstract void StopInternal();

    public abstract bool IsRunning();
}
