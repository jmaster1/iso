using System.Threading.Tasks;
using Common.Lang.Proxy;

namespace Common.IO.Transport.Rmi
{
    internal class Query
    {
        public int RequestId;
    
        public readonly TaskCompletionSource<object> TaskCompletionSource = 
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    
        public Task<object?> Task => TaskCompletionSource.Task;

        public MethodCall Call;

        public override string ToString()
        {
            return "requestId=" + RequestId + ", Call=" + Call;
        }
    }
}