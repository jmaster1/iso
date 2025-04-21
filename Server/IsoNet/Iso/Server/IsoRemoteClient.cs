using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Server;

public class IsoRemoteClient(AbstractTransport transport, ICodec<MethodCall> codec, IsoWorld world) : IIsoServerApi
{
    private IIsoClientApi remoteApi;
    
    MethodInvoker invoker = new();

    void Init()
    {
        (remoteApi, _) = Proxy.Create<IIsoClientApi>(call => transport.SendMessage(call, codec));
        
        invoker.Register<IIsoServerApi>(this);
        transport.SetMessageHandler(msg => invoker.Invoke(msg), codec);
    }

    public void CreateCells(int width, int height)
    {

    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Build(BuildingInfo buildingInfo, Cell cell, bool flip)
    {
        world.Buildings.Build(buildingInfo, cell, flip);
    }
}