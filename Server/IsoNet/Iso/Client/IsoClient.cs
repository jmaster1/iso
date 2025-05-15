using Common.TimeNS;
using Iso.Player;
using IsoNet.Core;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;
using Microsoft.Extensions.Logging;

namespace IsoNet.Iso.Client;

public class IsoClient(
    IsoPlayer player, 
    AbstractTransport transport, 
    ICodec codec, 
    Time? time = null) : LogAware
{
    public IsoPlayer Player => player;
    
    private readonly RunOnTime _runOnTime = new();

    public IIsoApi RemoteApi { get; private set; } = null!;
    
    public IIsoServerApi ServerApi { get; private set; } = null!;

    private TransportInvoker _invoker = null!;
    
    private Time? _time = time;

    public IsoClient Init()
    {
        if (_time == null)
        {
            _time = new Time();
            new TimeTimer().Start(_time, IsoCommon.Delta);
        }
        _runOnTime.FrameSupplier = () => Player.TimeGame.Frame;
        _runOnTime.Bind(_time);
        _invoker = new TransportInvoker(transport, codec).Init(call =>
        {
            var frame = call.GetAttr(IsoCommon.AttrFrame, Time.FrameUndefined);
            if (frame == Time.FrameUndefined)
            {
                _runOnTime.AddAction(() => _invoker.Invoke(call));
            }
            else
            {
                Logger?.LogInformation("Add call, frame={frame}, currentFrame={currentFrame}", 
                    frame, _runOnTime.Model.Frame);
                _runOnTime.AddAction(frame, () => _invoker.Invoke(call));    
            }
        });
        RemoteApi = _invoker.CreateRemote<IIsoApi>();
        ServerApi = _invoker.CreateRemote<IIsoServerApi>();
        _invoker.RegisterLocal<IIsoApi>(new IsoApi("client", player, _time));
        return this;
    }
}
