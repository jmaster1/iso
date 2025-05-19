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
    IsoWorld world, 
    AbstractTransport transport, 
    ICodec codec, 
    Time? time = null) : LogAware, IIsoClientApi
{
    public IsoWorld World => world;
    
    private readonly RunOnTime _runOnTime = new();

    public IIsoWorldApi RemoteWorldApi { get; private set; } = null!;
    
    private IIsoServerApi ServerApi;
    
    private Time? _time = time;
    
    public TransportRmi Rmi = null!;
    
    public readonly StringHolder WorldId = new();

    public IsoClient Init()
    {
        if (_time == null)
        {
            _time = new Time();
            new TimeTimer().Start(_time, IsoCommon.Delta);
        }
        _runOnTime.FrameSupplier = () => World.TimeGame.Frame;
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
        
        Rmi = new TransportRmi(transport, codec);
        RemoteWorldApi = Rmi.CreateRemote<IIsoWorldApi>();
        ServerApi = Rmi.CreateRemote<IIsoServerApi>();
        Rmi.RegisterLocal<IIsoWorldApi>(new IsoWorldApi(world));
        Rmi.RegisterLocal<IIsoClientApi>(this);
        return this;
    }

    public void CreateWorld(int width, int height)
    {
        var info = ServerApi.CreateWorld(width, height);
        world.Id = info.Id;
        world.Cells.Create(info.Width, info.Height);
        WorldId.Set(info.Id);
    }

    public void Start()
    {
        ServerApi.StartWorld();
    }

    public void WorldStarted()
    {
        world.Bind(_time);
    }
}
