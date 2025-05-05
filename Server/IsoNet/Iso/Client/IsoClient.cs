using Common.TimeNS;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;

namespace IsoNet.Iso.Client;

public class IsoClient(IsoPlayer player, AbstractTransport transport, ICodec<MethodCall> codec)
{
    public IsoPlayer Player => player;
    
    private readonly Time _time = new();
    //
    // private readonly TimeTimer _timeTimer = new();
    //
     private readonly RunOnTime _runOnTime = new();

    public IIsoApi RemoteApi { get; private set; } = null!;

    private TransportInvoker _invoker = null!;
    
    public IsoClient Init()
    {
        _invoker = new TransportInvoker(transport, codec).Init(call =>
        {
            var frame = call.GetAttr<int>(IsoCommon.AttrFrame);
            if (frame == Time.FrameUndefined)
            {
                _runOnTime.AddAction(() => _invoker.Invoke(call));
            }
            else
            {
                _runOnTime.AddAction(frame, () => _invoker.Invoke(call));    
            }
        });
        RemoteApi = _invoker.CreateRemote<IIsoApi>();
        _invoker.RegisterLocal<IIsoApi>(new IsoApi(player, _time));
        return this;
    }
}
