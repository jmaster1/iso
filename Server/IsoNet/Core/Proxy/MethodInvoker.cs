namespace IsoNet.Core.Proxy;

public class MethodInvoker
{
    private readonly Dictionary<Type, object> _targets = new();

    public void Register<T>(T target)
    {
        _targets[typeof(T)] = target!;        
    }

    public object? Invoke(MethodCall call)
    {
        var type = call.MethodInfo.ReflectedType;
        var target = _targets[type!];
        if (target is null)
        {
            throw new Exception("Target is null for type: " + type!.FullName);
        }
        return call.MethodInfo.Invoke(target, call.Args);
    }
}
