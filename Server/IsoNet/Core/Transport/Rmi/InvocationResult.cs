namespace IsoNet.Core.Transport.Rmi;

public class InvocationResult
{
    public object? Result { get; set; }
    
    public Exception? Exception { get; set; }
}