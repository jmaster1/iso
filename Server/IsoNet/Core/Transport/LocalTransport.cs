using IsoNet.Core.Transport.Server;

namespace IsoNet.Core.Transport;

using System.Threading.Channels;

public class LocalTransport : AbstractTransport
{
    private readonly Channel<MemoryStream> _inbox = Channel.CreateUnbounded<MemoryStream>();
    private LocalTransport? _peer;
    private bool _connected = true;

    public static (LocalTransport A, LocalTransport B) CreatePair()
    {
        var a = new LocalTransport();
        var b = new LocalTransport();

        a._peer = b;
        b._peer = a;

        a.StartReceiving();
        b.StartReceiving();

        return (a, b);
    }

    public static LocalServerTransport CreateServer(LocalTransport transport)
    {
        return new LocalServerTransport(transport);
    }

    private void StartReceiving()
    {
        _ = Task.Run(async () =>
        {
            await foreach (var stream in _inbox.Reader.ReadAllAsync())
            {
                try
                {
                    stream.Position = 0;
                    MessageReceived(stream);
                }
                finally
                {
                    stream.Dispose();
                }
            }
        });
    }

    protected override void DoWithOutput(Action<Stream> action)
    {
        if (_peer is not { _connected: true })
            throw new InvalidOperationException("Peer not connected");

        var ms = new MemoryStream();
        action(ms);
        ms.Position = 0;
        _peer._inbox.Writer.TryWrite(ms);
    }

    protected override Task DisconnectInternal()
    {
        _connected = false;
        _inbox.Writer.TryComplete();
        return Task.CompletedTask;
    }

    public override bool IsConnected() => _connected;
}

public class LocalServerTransport(LocalTransport clientTransport) : AbstractServer
{
    private bool _running;

    protected override void StartInternal()
    {
        _running = true;
        ClientConnected(clientTransport);
    }

    protected override void StopInternal()
    {
        _running = false;
    }

    public override bool IsRunning() => _running;
}
