using System.Collections.Concurrent;
using System.Reflection;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using MethodInvoker = IsoNet.Core.Proxy.MethodInvoker;

namespace IsoNet.Core.Transport.Rmi;

public class TransportRmi {
    
    private int _requestIdSeq;
    private readonly ConcurrentDictionary<int, TaskCompletionSource<object?>> _pendingRequests = new();
    private readonly AbstractTransport _transport;
    private readonly ICodec2 _codec;
    private readonly MethodInvoker _invoker;

    public TransportRmi(AbstractTransport transport, ICodec2 codec, MethodInvoker? invoker = null)
    {
        _transport = transport;
        _codec = codec;
        _invoker = invoker ?? new MethodInvoker();
        
        transport.SetMessageHandler(async stream =>
        {
            var reader = new BinaryReader(stream);
            var messageType = (MessageType)reader.ReadByte();

            switch (messageType)
            {
                case MessageType.Request:
                    var requestId = reader.ReadInt32();
                    var methodCall = codec.Read<MethodCall>(stream)!;
                    var result = _invoker.Invoke(methodCall);
                    if (result is Task task)
                    {
                        await task.ConfigureAwait(false);
                    }

                    transport.SendMessage(responseStream =>
                    {
                        var writer = new BinaryWriter(stream);
                        writer.Write((byte)MessageType.Response);
                        writer.Write(requestId);
                        codec.Write(result, responseStream);
                    });
                    break;
                case MessageType.Response:
                    requestId = reader.ReadInt32();
                    result = codec.Read<object>(stream);
                    if (_pendingRequests.TryRemove(requestId, out var tcs))
                    {
                        tcs.SetResult(result);
                    }
                    break;
                case MessageType.Call:
                    requestId = reader.ReadInt32();
                    methodCall = codec.Read<MethodCall>(stream)!;
                    _invoker.Invoke(methodCall);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
    }

    private int NextRequestId()
    {
        return Interlocked.Increment(ref _requestIdSeq);
    }
    
    public T CreateRemote<T>() where T : class
    {
        var (remoteApi, _) = Proxy.Proxy.Create<T>(async call =>
        {
            TaskCompletionSource<object?>? tcs = null;
            _transport.SendMessage(stream =>
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
                _codec.Write(call, stream);
            });
            if (tcs == null)
            {
                return null;
            }
            
            var result = await tcs.Task;

            var returnType = call.MethodInfo.ReturnType;
            var isTask = returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>);
            var resultType = isTask ? returnType.GetGenericArguments()[0] : null;
            if (isTask && resultType != null)
            {
                var casted = Convert.ChangeType(result, resultType);
                var taskResult = typeof(Task)
                    .GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(resultType)
                    .Invoke(null, new[] { casted });

                return taskResult!;
            }

            return result;
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

    public void RegisterLocal<T>(T api)
    {
        _invoker.Register<T>(api);
    }
}
