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
    
    private readonly Time _time = new();
    
    private readonly TimeTimer _timeTimer = new();
    
    private readonly RunOnTime _runOnTime = new();
    
    private IIsoApi _remoteApi = null!;

    private TransportRmi _invoker = null!;

    private readonly MethodInvoker _remoteInvoker = new();
    
    private IIsoClientApi _clientApi = null!;

    private WorldPlayers _worldPlayers = null!;

    internal IsoRemoteClient Init()
    {
        _invoker = new TransportRmi(transport, codec);
        _timeTimer.Start(_time, IsoCommon.Delta);
        _runOnTime.Bind(_time);
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
        _invoker.RegisterLocal<IIsoServerApi>(this);
        _remoteApi = _invoker.CreateRemote<IIsoApi>();
        /*call =>
        {
            call.SetAttr(IsoCommon.AttrFrame, Time.Frame);
        });
        */
        _clientApi = _invoker.CreateRemote<IIsoClientApi>();
        _remoteInvoker.Register(_remoteApi);
        return this;
    }

    public WorldInfo CreateWorld(int width, int height)
    {
        _worldPlayers = server.CreateWorld(width, height, this);
        return new WorldInfo
        {
            Id = World.Id,
            Width = width,
            Height = height
        };
    }

    public void StartWorld()
    {
        var local = new IsoApi(World, _time);
        _invoker.RegisterLocal<IIsoApi>(local);
    }

    public void JoinWorld(string worldId)
    {
        throw new NotImplementedException();
    }
}
