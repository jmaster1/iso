using Common.TimeNS;
using Iso.Player;
using IsoNet.Core.IO.Codec;
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
    public IsoWorld World => _worldPlayers.World;
    
    public Time Time => World.TimeGame;
    
    private IIsoWorldApi _remoteWorldApi = null!;

    public TransportRmi Rmi = null!;

    private readonly MethodInvoker _remoteInvoker = new();
    
    public IIsoClientApi _clientApi = null!;

    private WorldPlayers _worldPlayers = null!;

    internal IsoRemoteClient Init()
    {
        Rmi = new TransportRmi(transport, codec);
        
        
        //var local = new IsoApi(world, _time);
        
            /*.Init(call =>
        {
            _runOnTime.AddAction(() =>
            {
                _invoker.Invoke(call);
                if (call.MethodInfo.GetCustomAttribute<ReplayAttribute>() != null)
                {
                    _remoteInvoker.Invoke(call);
                }
            });
        });
        */
        Rmi.RegisterLocal<IIsoServerApi>(this);
        _remoteWorldApi = Rmi.CreateRemote<IIsoWorldApi>();
        /*call =>
        {
            call.SetAttr(IsoCommon.AttrFrame, Time.Frame);
        });
        */
        _clientApi = Rmi.CreateRemote<IIsoClientApi>();
        _remoteInvoker.Register(_remoteWorldApi);
        return this;
    }

    public void CreateWorld(int width, int height)
    {
        _worldPlayers = server.CreateWorld(width, height, this);
    }

    public void StartWorld()
    {
        server.StartWorld(_worldPlayers, this);
    }

    internal void WorldStarted()
    {
        var local = new IsoWorldApi(World);
        Rmi.RegisterLocal<IIsoWorldApi>(local);
        _clientApi.WorldStarted();
    }

    public void JoinWorld(string worldId)
    {
        throw new NotImplementedException();
    }
}
