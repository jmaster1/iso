using Common.TimeNS;
using Iso.Player;
using Common.IO.Codec;
using Common.Lang.Proxy;
using Common.IO.Transport;
using Common.IO.Transport.Rmi;
using Iso.Net.Common;
using MethodInvoker = Common.Lang.Proxy.MethodInvoker;

namespace IsoNet.Iso.Server;

public class IsoRemoteClient(
    IsoServer server,
    AbstractTransport transport, 
    ICodec codec) : IIsoServerApi
{
    public string? Id { get; set; }
    
    public IsoWorld World => _serverWorld.World;

    public readonly TransportRmi Rmi = new(transport, codec);

    internal readonly MethodInvoker RemoteInvoker = new();
    
    public IIsoClientApi ClientApi = null!;

    private ServerWorld _serverWorld = null!;

    internal IsoRemoteClient Init()
    {
        Rmi.RegisterLocal<IIsoServerApi>(this);
        Rmi.RemoteCallDecorator = DecorateCall;
        var remoteWorldApi = Rmi.CreateRemote<IIsoWorldApi>();
        RemoteInvoker.Register(remoteWorldApi); //???
        ClientApi = Rmi.CreateRemote<IIsoClientApi>();
        return this;
    }

    private void DecorateCall(MethodCall call)
    {
        call.Source = "server";
        call.Target = Id!;
        if (call.MethodInfo.DeclaringType == typeof(IIsoWorldApi))
        {
            call.SetFrame(World.TimeGame.Frame);
        }
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
        var localWorldApi = new IsoWorldApi(World);
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
