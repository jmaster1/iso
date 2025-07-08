using Common.TimeNS;
using Iso.Player;
using Iso.Serialize.Json;
using Iso.Net.Common;

namespace IsoNet.Iso.Server;

public class ServerWorld(IsoWorld world)
{
    private const int WorldFrameReportPeriod = 10;
    
    private readonly IsoWorldJsonSerializer _serializer = new(world);
    
    public IsoWorld World => world;
    
    public string Id => world.Id;

    public readonly DateTime Created = new();

    private readonly Time _time = new();
    
    private readonly TimerExecutor timerExecutor = new();
    
    private readonly RunOnTime _runOnTime = new();
    
    public readonly List<IsoRemoteClient> Clients = [];
    
    private int _nextReportFrame = 0;

    public void Start()
    {
        _runOnTime.Bind(_time);
        World.Bind(_time);
        timerExecutor.Start(IsoCommon.Delta, (delta) =>
        {
            _time.Update(delta);
            if (_time.Frame >= _nextReportFrame)
            {
                _nextReportFrame += WorldFrameReportPeriod;
                ForEachClient(cln => cln.ClientApi.WorldFrameReport(_time.Frame));
            }
        });
        ForEachClient(cln => cln.WorldStarted());
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

    public WorldInfo Join(IsoRemoteClient client)
    {
        Clients.Add(client);
        lock (world)
        {
            return new WorldInfo
            {
                Id = world.Id,
                State = _serializer.Export()
            };    
        }
    }
}
