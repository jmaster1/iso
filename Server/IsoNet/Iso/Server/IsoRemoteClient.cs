using Common.TimeNS;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Server;

public class IsoRemoteClient(
    AbstractTransport transport, 
    ICodec<MethodCall> codec, 
    IsoPlayer player)
{
    public IsoPlayer Player => player;
    
    private readonly Time _time = new();
    
    private readonly TimeTimer _timeTimer = new();
    
    private readonly RunOnTime _runOnTime = new();
    
    private IIsoApi _remoteApi = null!;

    private TransportInvoker _invoker = null!;

    internal IsoRemoteClient Init()
    {
        IsoApi local = new IsoApi(player, _time);
        _invoker = new TransportInvoker(transport, codec).Init(call =>
        {
            _runOnTime.AddAction(() => _invoker.Invoke(call));
        });
        _invoker.RegisterLocal<IIsoApi>(this);
        _remoteApi = _invoker.CreateRemote<IIsoApi>();
        return this;
    }

    public void CreateCells(int width, int height)
    {
        Player.Cells.Create(width, height, () =>
        {
            Player.Cells.ForEachPos((x, y) => Player.Cells.Set(x, y, CellType.Buildable));    
        });
        _remoteApi.CreateCells(width, height);
    }

    public void Start()
    {
        player.Bind(_time);
        _runOnTime.Bind(_time);
        _timeTimer.Start(_time, IsoCommon.Delta);
        _remoteApi.Start();
    }

    public void Build(BuildingInfo buildingInfo, Cell cell, bool flip)
    {
        _runOnTime.AddAction(() =>
        {
            player.Buildings.Build(buildingInfo, cell, flip);
            _remoteApi.Build(_time.Frame, buildingInfo, cell, flip);
        });
    }
}
