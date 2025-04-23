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
    IsoPlayer player) : IIsoServerApi
{
    private static readonly TimeSpan Delta = TimeSpan.FromMilliseconds(20);
    
    public IsoPlayer Player => player;
    
    private readonly Time _time = new();
    
    private IIsoClientApi _remoteApi = null!;

    private TransportInvoker _invoker = null!;

    internal IsoRemoteClient Init()
    {
        _invoker = new TransportInvoker(transport, codec).Init();
        _invoker.RegisterLocal<IIsoServerApi>(this);
        _remoteApi = _invoker.CreateRemote<IIsoClientApi>();
        return this;
    }

    public void CreateCells(int width, int height)
    {
        player.Cells.Create(width, height);
        _remoteApi.CreateCells(width, height);
    }

    public void Start()
    {
        player.Bind(_time);
        _time.StartTimer(Delta);
        _remoteApi.Start();
    }

    public void Build(BuildingInfo buildingInfo, Cell cell, bool flip)
    {
        _time.RunOnUpdate(() =>
        {
            player.Buildings.Build(buildingInfo, cell, flip);
            _remoteApi.Build(_time.Frame, buildingInfo, cell, flip);
        });
    }
}
