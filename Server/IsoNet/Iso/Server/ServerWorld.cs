using Common.TimeNS;
using Iso.Player;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Server;

public class ServerWorld(IsoWorld world)
{
    private const int WorldFrameReportPeriod = 10;
    
    public IsoWorld World => world;

    private readonly Time _time = new();
    
    public readonly TimeTimer TimeTimer = new();
    
    private readonly RunOnTime _runOnTime = new();
    
    public readonly List<IsoRemoteClient> Clients = [];
    
    private int _nextReportFrame = 0;

    public void Start()
    {
        _runOnTime.Bind(_time);
        TimeTimer.Start(_time, IsoCommon.Delta);
        World.Bind(_time);
        _time.AddListener(OnTimeUpdate);
        ForEachClient(cln => cln.WorldStarted());
    }

    private void OnTimeUpdate(Time time)
    {
        if (time.Frame >= _nextReportFrame)
        {
            _nextReportFrame += WorldFrameReportPeriod;
            ForEachClient(cln => cln.ClientApi.WorldFrameReport(time.Frame));
        }
    }

    public void ForEachClient(Action<IsoRemoteClient> action)
    {
        foreach (var cln in Clients)
        {
            action(cln);
        }
    }

    public void RunOnTime(Action action)
    {
        _runOnTime.AddAction(action);
    }
}
