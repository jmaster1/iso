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
    public string? Id { get; set; }
    
    public IsoWorld World => _serverWorld.World;
    
    public Time Time => World.TimeGame;
    
    public int Frame => Time.Frame;

    public readonly TransportRmi Rmi = new(transport, codec);

    internal readonly MethodInvoker RemoteInvoker = new();
    
    public IIsoClientApi ClientApi = null!;

    private ServerWorld _serverWorld = null!;
    
    private IsoWorldApi localWorldApi;

    internal IsoRemoteClient Init()
    {
        Rmi.RegisterLocal<IIsoServerApi>(this);
        
        var remoteWorldApi = Rmi.CreateRemote<IIsoWorldApi>(bean => bean.OnInvokeBefore = call =>
        {
            DecorateCall(call);
            call.SetFrame(Frame);
        });
        RemoteInvoker.Register(remoteWorldApi);
        ClientApi = Rmi.CreateRemote<IIsoClientApi>(bean => bean.OnInvokeBefore = DecorateCall);
        return this;
    }

    private void DecorateCall(MethodCall call)
    {
        call.Source = "server";
        call.Target = Id!;
    }

    public void ClientId(string id)
    {
        Id = id;
    }

    public string CreateWorld(int width, int height)
    {
        var serverWorld = server.CreateWorld(width, height);
        return serverWorld.Id;
    }
    
    public WorldInfo JoinWorld(string worldId)
    {
        _serverWorld = server.GetWorld(worldId);
        return _serverWorld.Join(this);
    }

    public void StartWorld()
    {
        _serverWorld.Start();
    }

    internal void WorldStarted()
    {
        localWorldApi = new IsoWorldApi(World);
        var (localProxy, localProxyBean) = Proxy.Create<IIsoWorldApi>(call =>
        {
            _serverWorld.RunOnTime(() =>
            {
                call.Invoke(localWorldApi);
                foreach (var isoRemoteClient in _serverWorld.Clients)
                {
                    isoRemoteClient.RemoteInvoker.Invoke(call);
                }
            });
            return null;
        });
        Rmi.RegisterLocal(localProxy);
        ClientApi.WorldStarted();
    }
}
