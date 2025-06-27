using Common.TimeNS;
using Iso.Player;
using Iso.Serialize.Json;
using IsoNet.Core;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Rmi;
using IsoNet.Iso.Common;
using Microsoft.Extensions.Logging;

namespace IsoNet.Iso.Client;

public class IsoClient(
    IsoWorld world, 
    AbstractTransport transport, 
    ICodec codec, 
    Time time) : LogAware
{
    public IsoWorld World => world;
    
    public Time WorldTime => world.TimeGame;
    
    private readonly RunOnTime _runOnTime = new();

    public IIsoWorldApi RemoteWorldApi { get; private set; } = null!;
    
    public string? Id { get; set; }

    private IIsoServerApi _serverApi;
    
    public TransportRmi Rmi = null!;

    private int _lastFrameReported;
    
    private readonly IsoWorldJsonSerializer _serializer = new(world);

    public IsoClient Init()
    {
        _runOnTime.FrameSupplier = () => World.TimeGame.Frame;
        _runOnTime.Bind(time);
        time.AddListener(OnTimeUpdate);
        
        Rmi = new TransportRmi(transport, codec)
        {
            CallExecutor = (call, action) =>
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
                call =>
                {
                    DecorateCall(call);
                    call.SetFrame(World.TimeGame.Frame);
                });
        _serverApi = Rmi.CreateRemote<IIsoServerApi>(
            proxyBean => proxyBean.OnInvokeBefore = 
                call =>
                {
                    DecorateCall(call);
                });

        Rmi.RegisterLocal<IIsoWorldApi>(new IsoWorldApi(world));
        Rmi.RegisterLocal<IIsoClientApi>(new IsoClientApi(this));
        return this;
    }

    private void DecorateCall(MethodCall call)
    {
        call.Source = Id;
        call.Target = "server";
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

    public string CreateWorld(int width, int height)
    {
        return _serverApi.CreateWorld(width, height);
    }
    
    public WorldInfo JoinWorld(string worldId)
    {
        var info = _serverApi.JoinWorld(worldId);
        world.Id = info.Id;
        _serializer.Import(info.State);
        // world.Cells.Create(info.Width, info.Heigth, () =>
        // {
        //     world.Cells.ForEachPos((x, y) => world.Cells.Set(x, y, CellType.Buildable));    
        // });
        return info;
    }

    public void StartWorld()
    {
        _serverApi.StartWorld();
    }

    internal void WorldStarted()
    {
        world.Bind(time);
    }

    internal void WorldFrameReport(int frame)
    {
        _lastFrameReported = frame;
        Logger?.LogInformation("WorldFrameReport: {frame}", frame);
    }

    public async Task Disconnect()
    {
        await transport.Disconnect();
    }
}
