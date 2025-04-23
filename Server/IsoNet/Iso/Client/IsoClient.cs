using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Client;

public class IsoClient(AbstractTransport transport, ICodec<MethodCall> codec) : IIsoClientApi
{
    public readonly IsoPlayer Player = new();
    
    private IIsoServerApi _remoteApi = null!;
    
    public IIsoServerApi RemoteApi => _remoteApi;

    private TransportInvoker _invoker = null!;
    
    public IsoClient Init()
    {
        _invoker = new TransportInvoker(transport, codec).Init();
        _invoker.RegisterLocal<IIsoClientApi>(this);
        _remoteApi = _invoker.CreateRemote<IIsoServerApi>();
        return this;
    }

    public void CreateCells(int width, int height)
    {
        Player.Cells.Create(width, height);
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Build(int frame, BuildingInfo buildingInfo, Cell cell, bool flip)
    {
        throw new NotImplementedException();
    }
}