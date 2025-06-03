using Common.TimeNS;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Rmi;
using IsoNet.Iso.Common;
using MethodInvoker = IsoNet.Core.Proxy.MethodInvoker;

namespace IsoNet.Iso.Server;

public class IsoRemoteClient(
    IsoServer server,
    AbstractTransport transport, 
    ICodec codec) : IIsoServerApi
{
    public IsoWorld World => _serverWorld.World;
    
    public Time Time => World.TimeGame;
    
    public int Frame => Time.Frame;

    public readonly TransportRmi Rmi = new(transport, codec);

    internal readonly MethodInvoker RemoteInvoker = new();
    
    public IIsoClientApi ClientApi = null!;

    private ServerWorld _serverWorld = null!;
    
    private IsoWorldApi local;

    internal IsoRemoteClient Init()
    {
        Rmi.RegisterLocal<IIsoServerApi>(this);
        
        var remoteWorldApi = Rmi.CreateRemote<IIsoWorldApi>();
        RemoteInvoker.Register(remoteWorldApi);
        
        ClientApi = Rmi.CreateRemote<IIsoClientApi>();
        return this;
    }

    public void CreateWorld(int width, int height)
    {
        _serverWorld = server.CreateWorld(width, height, this);
    }

    public void StartWorld()
    {
        server.StartWorld(_serverWorld, this);
    }

    internal void WorldStarted()
    {
        local = new IsoWorldApi(World);
        var (localProxy, localProxyBean) = Proxy.Create<IIsoWorldApi>(
            HandleIsoWorldApiCall);
        localProxyBean.OnInvokeBefore = call => call.SetAttr(IsoCommon.AttrFrame, Frame);
        Rmi.RegisterLocal(localProxy);
        ClientApi.WorldStarted();
    }

    private object? HandleIsoWorldApiCall(MethodCall call)
    {
        _serverWorld.RunOnTime(() =>
        {
            var result = call.MethodInfo.Invoke(local, call.Args);
            foreach (var isoRemoteClient in _serverWorld.Clients)
            {
                isoRemoteClient.RemoteInvoker.Invoke(call);
            }
        });
        return null;
    }

    public void JoinWorld(string worldId)
    {
        throw new NotImplementedException();
    }
}
