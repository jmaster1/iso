using Common.TimeNS;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Client;

public class IsoClient(IsoPlayer player, AbstractTransport transport, ICodec<MethodCall> codec)
{
    public IsoPlayer Player => player;
    
    private readonly Time _time = new();
    
    private readonly TimeTimer _timeTimer = new();
    
    private readonly RunOnTime _runOnTime = new();

    public IIsoApi RemoteApi { get; private set; } = null!;

    private TransportInvoker _invoker = null!;
    
    public IsoClient Init()
    {
        _invoker = new TransportInvoker(transport, codec).Init();
        _invoker.RegisterLocal<IIsoApi>(this);
        RemoteApi = _invoker.CreateRemote<IIsoApi>();
        return this;
    }

    public void CreateCells(int width, int height)
    {
        Player.Cells.Create(width, height, () =>
        {
            Player.Cells.ForEachPos((x, y) => Player.Cells.Set(x, y, CellType.Buildable));    
        });
    }

    public void Start()
    {
        Player.Bind(_time);
        _runOnTime.Bind(_time);
        _timeTimer.Start(_time, IsoCommon.Delta);
    }

    public void Build(int frame, BuildingInfo buildingInfo, Cell cell, bool flip)
    {
        _runOnTime.AddAction(frame, () =>
        {
            Player.Buildings.Build(buildingInfo, cell, flip);    
        });
    }
}
