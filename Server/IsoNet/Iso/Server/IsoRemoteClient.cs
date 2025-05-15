using System.Reflection;
using Common.TimeNS;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;
using MethodInvoker = IsoNet.Core.Proxy.MethodInvoker;

namespace IsoNet.Iso.Server;

public class IsoRemoteClient(
    IsoServer server,
    AbstractTransport transport, 
    ICodec codec, 
    IsoPlayer player) : IIsoServerApi
{
    public IsoPlayer Player => player;
    
    public Time Time => player.TimeGame;
    
    private readonly Time _time = new();
    
    private readonly TimeTimer _timeTimer = new();
    
    private readonly RunOnTime _runOnTime = new();
    
    private IIsoApi _remoteApi = null!;

    private TransportInvoker _invoker = null!;

    private readonly MethodInvoker _remoteInvoker = new();
    
    private IIsoClientApi _clientApi = null!;

    private IsoPlayer _world;

    internal IsoRemoteClient Init()
    {
        _timeTimer.Start(_time, IsoCommon.Delta);
        _runOnTime.Bind(_time);
        var local = new IsoApi("server", player, _time);
        _invoker = new TransportInvoker(transport, codec).Init(call =>
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
        _invoker.RegisterLocal<IIsoApi>(local);
        _invoker.RegisterLocal<IIsoServerApi>(this);
        _remoteApi = _invoker.CreateRemote<IIsoApi>(call =>
        {
            call.SetAttr(IsoCommon.AttrFrame, Time.Frame);
        });
        _clientApi = _invoker.CreateRemote<IIsoClientApi>();
        _remoteInvoker.Register(_remoteApi);
        return this;
    }

    public void CreateWorld()
    {
        _world = server.CreateWorld();
        _clientApi.WorldCreated(_world.Guid);
    }

    public void JoinWorld(string worldId)
    {
        throw new NotImplementedException();
    }
}
