using System.Collections.Concurrent;
using System.Reflection;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using MethodInvoker = IsoNet.Core.Proxy.MethodInvoker;

namespace IsoNet.Core.Transport.Rmi;

public class TransportRmi {
    
    private int _requestIdSeq;
    private readonly ConcurrentDictionary<int, Query> _pendingRequests = new();
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
        if (!_pendingRequests.TryRemove(requestId, out var query)) return;
        var exceptionOccured = reader.ReadBoolean();
        if (exceptionOccured)
        {
            var exception = _codec.Read<Exception>(reader.BaseStream);
            query.TaskCompletionSource.SetException(exception!);
        }
        else
        {
            var resultType = query.Call.MethodInfo.ReturnType;
            if (IsGenericTask(resultType, out var taskGenericType))
            {
                resultType = taskGenericType;
            }
            var result = _codec.Read(reader.BaseStream, resultType);
            query.TaskCompletionSource.SetResult(result);  
        }
    }

    private async Task ReadRequest(BinaryReader reader)
    {
        var requestId = reader.ReadInt32();
        var methodCall = _codec.Read<MethodCall>(reader.BaseStream)!;
        object? result = null;
        Exception? exception = null;
        try
        {
            result = _invoker.Invoke(methodCall);
        }
        catch (TargetInvocationException e)
        {
            exception = e.InnerException;
        }
        if (result is Task task)
        {
            await task.ConfigureAwait(false);
                        
            var taskType = task.GetType();
            if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultProperty = taskType.GetProperty("Result");
                result = resultProperty?.GetValue(task);
            }
            else
            {
                result = null;
            }
        }
        var exceptionOccured = exception is not null;
        WriteMessage(MessageType.Response, requestId, exceptionOccured ? exception : result,
            writer => writer.Write(exceptionOccured));
    }

    private void WriteMessage(MessageType messageType, int requestId, object? result,
        Action<BinaryWriter>? writeBeforeResult = null)
    {
        _transport.SendMessage(responseStream =>
        {
            var writer = new BinaryWriter(responseStream);
            writer.Write((byte)messageType);
            writer.Write(requestId);
            writeBeforeResult?.Invoke(writer);
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
            Query? query = null;
            var requestId = NextRequestId();
            if (messageType == MessageType.Request)
            {
                query = _pendingRequests[requestId] = new Query
                {
                    RequestId = requestId,
                    Call = call
                };
            }
            
            WriteMessage(messageType, requestId, call);
            
            if (query == null)
            {
                return null;
            }
            
            var returnType = call.MethodInfo.ReturnType;
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType == typeof(Task))
                {
                    return query.Task;
                }

                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var resultType = returnType.GetGenericArguments()[0];
                    return ReturnAsync(resultType, query.Task);
                }
            }

            try
            {
                query.Task.Wait();
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException!;
            }

            return Convert.ChangeType(query.Task.Result, returnType);
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
    
    static bool IsGenericTask(Type type, out Type genericArgument)
    {
        genericArgument = null;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            genericArgument = type.GetGenericArguments()[0];
            return true;
        }

        return false;
    }
}
