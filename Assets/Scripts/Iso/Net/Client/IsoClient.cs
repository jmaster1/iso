using System.Threading.Tasks;
using Common.TimeNS;
using Iso.Player;
using Iso.Serialize.Json;
using IsoNet.Core;
using Common.IO.Codec;
using Common.Lang.Proxy;
using Common.IO.Transport;
using Common.IO.Transport.Rmi;
using Common.IO.Transport.WebSocket;
using Iso.Net.Common;
using Iso.Net.Common.Json;
using Microsoft.Extensions.Logging;

namespace Iso.Net.Client
{
    public class IsoClient : LogAware
    {
        public IsoWorld World => _world;
    
        public Time WorldTime => _world.TimeGame;
    
        private readonly RunOnTime _runOnTime = new();

        public IIsoWorldApi RemoteWorldApi { get; private set; } = null!;
    
        public string? Id { get; set; }

        private IIsoServerApi _serverApi;
        
        public IIsoServerApi ServerApi => _serverApi;
    
        public TransportRmi Rmi = null!;

        private int _lastFrameReported;
    
        private readonly IsoWorldJsonSerializer _serializer;
        private readonly IsoWorld _world;
        private readonly AbstractTransport _transport;
        private readonly ICodec _codec;
        private readonly Time _time;

        public static async Task<IsoClient> CreateWebsocket(IsoWorld world, Time time,
            string url = "ws://localhost:7000/ws/")
        {
            var transport = new WebSocketClient();
            await transport.Connect(url);
            var codec = IsoJsonCodecFactory.CreateCodec().WrapLogging(transport.Logger);
            var client = new IsoClient(world, transport, codec, time);
            client.Init();
            return client;
        }

        public IsoClient(IsoWorld world, 
            AbstractTransport transport, 
            ICodec codec, 
            Time time)
        {
            _world = world;
            _transport = transport;
            _codec = codec;
            _time = time;
            _serializer = new IsoWorldJsonSerializer(world);
        }

        public IsoClient Init()
        {
            _runOnTime.FrameSupplier = () => World.TimeGame.Frame;
            _runOnTime.Bind(_time);
            _time.AddListener(OnTimeUpdate);
        
            Rmi = new TransportRmi(_transport, _codec)
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
                },
                RemoteCallDecorator = DecorateCall
            };

            RemoteWorldApi = Rmi.CreateRemote<IIsoWorldApi>();
            _serverApi = Rmi.CreateRemote<IIsoServerApi>();

            Rmi.RegisterLocal<IIsoWorldApi>(new IsoWorldApi(_world));
            Rmi.RegisterLocal<IIsoClientApi>(new IsoClientApi(this));
            return this;
        }

        private void DecorateCall(MethodCall call)
        {
            call.Source = Id;
            call.Target = "server";
            if (call.MethodInfo.DeclaringType == typeof(IIsoWorldApi))
            {
                call.SetFrame(World.TimeGame.Frame);
            }
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
    
        public void JoinWorld(string worldId)
        {
            var info = _serverApi.JoinWorld(worldId);
            _world.Id = info.Id;
            _serializer.Import(info.State);
        }

        public void StartWorld()
        {
            _serverApi.StartWorld();
        }

        internal void WorldStarted()
        {
            _world.Bind(_time);
        }

        internal void WorldFrameReport(int frame)
        {
            _lastFrameReported = frame;
            Logger?.LogInformation("WorldFrameReport: {frame}", frame);
        }

        public async Task Disconnect()
        {
            await _transport.Disconnect();
        }
    }
}
