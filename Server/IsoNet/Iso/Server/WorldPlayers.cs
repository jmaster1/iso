using Common.TimeNS;
using Iso.Player;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Server;

public class WorldPlayers
{
    public IsoWorld World;

    private readonly Time _time = new();
    
    public readonly TimeTimer _timeTimer = new();
    
    private readonly RunOnTime _runOnTime = new();
    
    public List<IsoRemoteClient> Clients = [];

    public void Start()
    {
        _runOnTime.Bind(_time);
        _timeTimer.Start(_time, IsoCommon.Delta);
        World.Bind(_time);
    }
}
