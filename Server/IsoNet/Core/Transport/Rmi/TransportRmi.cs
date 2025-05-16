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
    private readonly ICodec _codec;
    private readonly MethodInvoker _invoker;

    public TransportRmi(AbstractTransport transport, ICodec codec, MethodInvoker? invoker = null)
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
                    await ReadRequest(reader);
                    break;
                case MessageType.Response:
                    ReadResponse(reader);
                    break;
                case MessageType.Call:
                    ReadCall(reader);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
    }

    private void ReadCall(BinaryReader reader)
    {
        var requestId = reader.ReadInt32();
        var methodCall = _codec.Read<MethodCall>(reader.BaseStream)!;
        _invoker.Invoke(methodCall);
    }

    private void ReadResponse(BinaryReader reader)
    {
        var requestId = reader.ReadInt32();
        var result = _codec.Read<InvocationResult>(reader.BaseStream)!;
        if (!_pendingRequests.TryRemove(requestId, out var tcs)) return;
        if (result.Exception != null)
        {
            tcs.SetException(result.Exception!);    
        }
        else
        {
            tcs.SetResult(result.Result);  
        }
    }

    private async Task ReadRequest(BinaryReader reader)
    {
        var requestId = reader.ReadInt32();
        var methodCall = _codec.Read<MethodCall>(reader.BaseStream)!;
        var result = new InvocationResult();
        try
        {
            result.Result = _invoker.Invoke(methodCall);
        }
        catch (TargetInvocationException e)
        {
            result.Exception = e.InnerException;
        }
        if (result.Result is Task task)
        {
            await task.ConfigureAwait(false);
                        
            var taskType = task.GetType();
            if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultProperty = taskType.GetProperty("Result");
                result.Result = resultProperty?.GetValue(task);
            }
            else
            {
                result.Result = null;
            }
        }

        _transport.SendMessage(responseStream =>
        {
            var writer = new BinaryWriter(responseStream);
            writer.Write((byte)MessageType.Response);
            writer.Write(requestId);
            _codec.Write(result, responseStream);
        });
    }

    private int NextRequestId()
    {
        return Interlocked.Increment(ref _requestIdSeq);
    }
    
    public T CreateRemote<T>() where T : class
    {
        var (remoteApi, _) = Proxy.Proxy.Create<T>(call =>
        {
            var messageType = ResolveMessageType(call.MethodInfo);
            TaskCompletionSource<object?>? tcs = null;
            var requestId = NextRequestId();
            if (messageType == MessageType.Request)
            {
                tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
                _pendingRequests[requestId] = tcs;
            }
            
            _transport.SendMessage(stream =>
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)messageType);
                writer.Write(requestId);
                _codec.Write(call, stream);
            });
            
            if (tcs == null)
            {
                return null;
            }
            
            var returnType = call.MethodInfo.ReturnType;
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType == typeof(Task))
                {
                    return tcs.Task;
                }

                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var resultType = returnType.GetGenericArguments()[0];
                    return ReturnAsync(resultType, tcs!.Task);
                }
            }

            tcs.Task.Wait();
            return Convert.ChangeType(tcs.Task.Result, returnType);
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
        _invoker.Register(api);
    }
    
    private static object ReturnAsync(Type resultType, Task<object?> task)
    {
        // Generic method -> Task<T>
        var method = typeof(TransportRmi)
            .GetMethod(nameof(ReturnAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(resultType);

        return method.Invoke(null, [task])!;
    }

    private static async Task<T> ReturnAsyncGeneric<T>(Task<object?> task)
    {
        var result = await task.ConfigureAwait(false);
        return (T)Convert.ChangeType(result, typeof(T))!;
    }
}
