using Common.TimeNS;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Server;

public class IsoRemoteClient(
    AbstractTransport transport, 
    ICodec<MethodCall> codec, 
    IsoPlayer player)
{
    public IsoPlayer Player => player;
    
    public Time Time => player.TimeGame;
    
    private readonly Time _time = new();
    
    private readonly TimeTimer _timeTimer = new();
    
    private readonly RunOnTime _runOnTime = new();
    
    private IIsoApi _remoteApi = null!;

    private TransportInvoker _invoker = null!;

    private readonly MethodInvoker _remoteInvoker = new();

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
                _remoteInvoker.Invoke(call);
            });
        });
        _invoker.RegisterLocal<IIsoApi>(local);
        _remoteApi = _invoker.CreateRemote<IIsoApi>(call =>
        {
            call.SetAttr(IsoCommon.AttrFrame, Time.Frame);
        });
        _remoteInvoker.Register(_remoteApi);
        return this;
    }
}
