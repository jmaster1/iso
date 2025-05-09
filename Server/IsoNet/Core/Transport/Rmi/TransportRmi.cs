using System.Collections.Concurrent;
using System.Reflection;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using MethodInvoker = IsoNet.Core.Proxy.MethodInvoker;

namespace IsoNet.Core.Transport.Rmi;

public class TransportRmi(
    AbstractTransport transport, 
    ICodec<MethodCall> methodCallCodec,
    ICodec<object?> responseCodec,
    MethodInvoker invoker)
{
    private int _requestIdSeq;
    
    private readonly ConcurrentDictionary<int, TaskCompletionSource<object?>> _pendingRequests = new();

    private int NextRequestId()
    {
        return Interlocked.Increment(ref _requestIdSeq);
    }
    
    public TransportRmi Init()
    {
        transport.SetMessageHandler(stream =>
        {
            var reader = new BinaryReader(stream);
            var messageType = (MessageType)reader.ReadByte();

            switch (messageType)
            {
                case MessageType.Request:
                    var requestId = reader.ReadInt32();
                    var methodCall = methodCallCodec.Read(stream);
                    var result = invoker.Invoke(methodCall);
                    transport.SendMessage(responseStream =>
                    {
                        var writer = new BinaryWriter(stream);
                        writer.Write((byte)MessageType.Response);
                        writer.Write(requestId);
                        responseCodec.Write(result, responseStream);
                    });
                    break;
                case MessageType.Response:
                    requestId = reader.ReadInt32();
                    result = responseCodec.Read(stream);
                    if (_pendingRequests.TryRemove(requestId, out var tcs))
                    {
                        tcs.SetResult(result);
                    }
                    break;
                case MessageType.Call:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
        return this;
    }
    
    public T CreateRemote<T>() where T : class
    {
        var (remoteApi, _) = Proxy.Proxy.Create<T>(async call =>
        {
            TaskCompletionSource<object?>? tcs = null;
            transport.SendMessage(stream =>
            {
                var writer = new BinaryWriter(stream);
                var messageType = ResolveMessageType(call.MethodInfo);
                var requestId = NextRequestId();
                if (messageType == MessageType.Request)
                {
                    tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _pendingRequests[requestId] = tcs;
                }
                writer.Write((byte)messageType);
                writer.Write(requestId);
                methodCallCodec.Write(call, stream);
            });
            return tcs == null ? null : await tcs.Task;
        });
        return remoteApi;
    }

    private MessageType ResolveMessageType(MethodInfo methodInfo)
    {
        if(methodInfo.GetCustomAttribute<QueryAttribute>() != null) return MessageType.Request;
        if(methodInfo.GetCustomAttribute<CallAttribute>() != null) return MessageType.Call;
        if(methodInfo.DeclaringType!.GetCustomAttribute<QueryAttribute>() != null) return MessageType.Request;
        if(methodInfo.DeclaringType!.GetCustomAttribute<CallAttribute>() != null) return MessageType.Call;
        return MessageType.Request;

    }
}