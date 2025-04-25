using System.Reflection;

namespace IsoNet.Core.Proxy;

public class MethodCall
{
    public MethodInfo MethodInfo = null!;
    
    public object?[]? Args;
}