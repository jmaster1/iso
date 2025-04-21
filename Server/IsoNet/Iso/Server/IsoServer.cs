using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Iso.Common;
using IsoNet.Server;

namespace IsoNet.Iso.Server;

public class IsoServer(AbstractServer server, ICodec<MethodCall> codec)
{
    void Init()
    {
        server.OnClientConnected += transport =>
        {
            InitTransport(transport);
            // var (serverRemoteApi, serverLocalApi) = InitTransport(transport);
            // pingFromServer = async message =>
            // {
            //     await Ping(transport, serverRemoteApi, serverLocalApi, message);
            // };
        };
    }

    private void InitTransport(AbstractTransport transport)
    {
        IsoRemoteClient client = new IsoRemoteClient(transport, codec, new IsoWorld());
        var (remoteApi, _) = Proxy.Create<IIsoClientApi>(call => transport.SendMessage(call, codec));
    }
}