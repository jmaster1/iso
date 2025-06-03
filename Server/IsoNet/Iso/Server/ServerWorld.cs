using Common.TimeNS;
using Iso.Player;
using IsoNet.Core.Proxy;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Server;

public class ServerWorld(IsoWorld world)
{
    public IsoWorld World => world;

    private readonly Time _time = new();
    
    public readonly TimeTimer TimeTimer = new();
    
    private readonly RunOnTime _runOnTime = new();
    
    public readonly List<IsoRemoteClient> Clients = [];

    public void Start()
    {
        _runOnTime.Bind(_time);
        TimeTimer.Start(_time, IsoCommon.Delta);
        World.Bind(_time);
    }

    public void ForEachClient(Action<IsoRemoteClient> action)
    {
        foreach (var cln in Clients)
        {
            action(cln);
        }
    }

    public void OnIsoWorldApiCallAfter(MethodCall call, object? result, Exception? error)
    {
        if (error != null) return;
        foreach (var isoRemoteClient in Clients)
        {
            isoRemoteClient.RemoteInvoker.Invoke(call);
        }
    }
}
