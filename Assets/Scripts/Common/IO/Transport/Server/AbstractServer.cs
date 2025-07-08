using System;
using IsoNet.Core;
using Microsoft.Extensions.Logging;

namespace Common.IO.Transport.Server
{
    public abstract class AbstractServer : LogAware
    {
        public event Action<AbstractTransport>? OnClientConnected;
    
        public int ConnectCount { get; private set; }
    
        public int DisconnectCount { get; private set; }

        public int CountActive => ConnectCount - DisconnectCount;
    
        protected void ClientConnected(AbstractTransport transport)
        {
            ConnectCount++;
            transport.Logger = Logger;
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
}
