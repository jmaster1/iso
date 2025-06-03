using System.Reflection;

namespace IsoNet.Core.Proxy;

public class ProxyBean<T> : DispatchProxy where T : class
{
    public Func<MethodCall, object?>? OnInvoke;
    
    public Action<MethodCall, object?, Exception?>? OnInvokeAfter;
    
    public Action<MethodCall>? OnInvokeBefore;
    
    public T? Target { get; set; }
    
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        var methodCall = new MethodCall
        {
            MethodInfo = targetMethod!,
            Args = args
        };

        object? result = null;
        OnInvokeBefore?.Invoke(methodCall);
        Exception? error = null;
        try
        {
            if (OnInvoke != null)
            {
                result = OnInvoke.Invoke(methodCall);
            }

            if (Target != null)
            {
                result = targetMethod!.Invoke(Target, args);
            }
        }
        catch (Exception ex)
        {
            error = ex;
            throw;
        }
        finally
        {
            OnInvokeAfter?.Invoke(methodCall, result, error);    
        }
        return result;
    }
}

public static class Proxy
{
    public static (T Proxy, ProxyBean<T> Bean) Create<T>(Func<MethodCall, object?>? handler = null, T? target = null) where T : class
    {
        if (!typeof(T).IsInterface)
            throw new InvalidOperationException($"{typeof(T).Name} must be an interface");

        var proxy = DispatchProxy.Create<T, ProxyBean<T>>();
        var bean = (ProxyBean<T>)(object)proxy;
        bean.Target = target;
        if (handler != null)
        {
            bean.OnInvoke = handler;
        }
        return (proxy, bean);
    }
    
    public static (T Proxy, ProxyBean<T> Bean) Create<T>(T target)
        where T : class =>
        Create(null, target);
}
