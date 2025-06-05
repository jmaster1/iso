using Common.Lang.Observable;
using Common.TimeNS;
using Iso.Cells;
using Iso.Player;
using IsoNet.Core;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Rmi;
using IsoNet.Iso.Common;
using Microsoft.Extensions.Logging;

namespace IsoNet.Iso.Client;

public class IsoClient(
    IsoWorld world, 
    AbstractTransport transport, 
    ICodec codec, 
    Time time) : LogAware, IIsoClientApi
{
    public IsoWorld World => world;
    public Time WorldTime => world.TimeGame;
    
    private readonly RunOnTime _runOnTime = new();

    public IIsoWorldApi RemoteWorldApi { get; private set; } = null!;
    
    private IIsoServerApi _serverApi;
    
    public TransportRmi Rmi = null!;
    
    public readonly StringHolder WorldId = new();

    private int _lastFrameReported;

    public IsoClient Init()
    {
        _runOnTime.FrameSupplier = () => World.TimeGame.Frame;
        _runOnTime.Bind(time);
        time.AddListener(OnTimeUpdate);
        
        Rmi = new TransportRmi(transport, codec)
        {
            CallRunner = (call, action) =>
            {
                var frame = call.GetFrame();
                if (frame == Time.FrameUndefined)
                {
                    _runOnTime.AddAction(action);    
                }
                else
                {
                    _runOnTime.AddAction(frame, action);    
                }
            }
        };

        RemoteWorldApi = Rmi.CreateRemote<IIsoWorldApi>(
            proxyBean => proxyBean.OnInvokeBefore = 
                call => call.SetFrame(World.TimeGame.Frame));
        _serverApi = Rmi.CreateRemote<IIsoServerApi>();

        var worldApi = new IsoWorldApi(world);
        Rmi.RegisterLocal<IIsoWorldApi>(worldApi);
        Rmi.RegisterLocal<IIsoClientApi>(this);
        return this;
    }

    private void OnTimeUpdate(Time _)
    {
        var updateLock = WorldTime.UpdateLock;
        if (!updateLock.Value && WorldTime.Frame >= _lastFrameReported)
        {
            updateLock.AddLock(this);
        } else if (updateLock.Value && WorldTime.Frame < _lastFrameReported)
        {
            updateLock.RemoveLock(this);
        }
    }

    public void CreateWorld(int width, int height)
    {
        _serverApi.CreateWorld(width, height);
    }

    public void Start()
    {
        _serverApi.StartWorld();
    }

    public void WorldСreated(WorldInfo info)
    {
        world.Id = info.Id;
        world.Cells.Create(info.Width, info.Height, () =>
        {
            world.Cells.ForEachPos((x, y) => world.Cells.Set(x, y, CellType.Buildable));    
        });
        WorldId.Set(info.Id);
    }

    public void WorldStarted()
    {
        world.Bind(time);
    }

    public void WorldFrameReport(int frame)
    {
        _lastFrameReported = frame;
        Logger?.LogInformation("WorldFrameReport: {frame}", frame);
    }
}
