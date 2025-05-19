using Common.Lang.Observable;
using Common.TimeNS;
using Iso.Player;
using IsoNet.Core;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Rmi;
using IsoNet.Iso.Common;

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
    
    private IIsoServerApi ServerApi;

    
    private Time? _time = time;
    
    private TransportRmi _transportRmi = null!;
    
    public readonly StringHolder WorldId = new();

    public IsoClient Init()
    {
        if (_time == null)
        {
            _time = new Time();
            new TimeTimer().Start(_time, IsoCommon.Delta);
        }
        _runOnTime.FrameSupplier = () => Player.TimeGame.Frame;
        _runOnTime.Bind(_time);
        /*
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
        */
        
        _transportRmi = new TransportRmi(transport, codec);
        RemoteApi = _transportRmi.CreateRemote<IIsoApi>();
        ServerApi = _transportRmi.CreateRemote<IIsoServerApi>();
        _transportRmi.RegisterLocal<IIsoApi>(new IsoApi("client", player, _time));
        return this;
    }

    public void CreateWorld(int width, int height)
    {
        var info = ServerApi.CreateWorld(width, height);
        player.Id = info.Id;
        player.Cells.Create(info.Width, info.Height);
        WorldId.Set(info.Id);
    }
}
