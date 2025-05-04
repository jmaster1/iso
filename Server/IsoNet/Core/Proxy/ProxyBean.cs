using System.Reflection;

namespace IsoNet.Core.Proxy;

public class ProxyBean<T> : DispatchProxy where T : class
{
    public event Action<MethodCall>? OnInvoke;
    
    public T? Target { get; set; }
    
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (OnInvoke != null)
        {
            var methodCall = new MethodCall
            {
                MethodInfo = targetMethod!,
                Args = args
            };
            OnInvoke.Invoke(methodCall);
        }

        if (Target != null)
        {
            targetMethod!.Invoke(Target, args);
        }

        return null;
    }
}

public static class Proxy
{
    public static (T Proxy, ProxyBean<T> Bean) Create<T>(Action<MethodCall>? handler = null, T? target = null) where T : class
    {
        if (!typeof(T).IsInterface)
            throw new InvalidOperationException($"{typeof(T).Name} must be an interface");

        var proxy = DispatchProxy.Create<T, ProxyBean<T>>();
        var bean = (ProxyBean<T>)(object)proxy;
        bean.Target = target;
        if (handler != null)
        {
            bean.OnInvoke += handler;
        }
        return (proxy, bean);
    }
}
