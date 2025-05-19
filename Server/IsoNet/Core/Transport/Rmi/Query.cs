using IsoNet.Core.Proxy;

namespace IsoNet.Core.Transport.Rmi;

public class Query
{
    public int RequestId;
    
    public readonly TaskCompletionSource<object?> TaskCompletionSource = 
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    
    public Task<object?> Task => TaskCompletionSource.Task;

    public MethodCall Call;
}