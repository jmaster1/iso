using System;
using System.Reflection;

namespace Common.Lang.Proxy
{
    public class ProxyBean<T> : DispatchProxy where T : class
    {
        public Func<MethodCall, object>? Executor;
    
        public event Action<MethodCall, object?, Exception?>? OnInvokeAfter;
    
        public event Action<MethodCall>? OnInvokeBefore;
    
        public T? Target { get; set; }
    
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            var call = new MethodCall
            {
                MethodInfo = targetMethod!,
                Args = args
            };

            object? result = null;
            OnInvokeBefore?.Invoke(call);
            Exception? error = null;
            try
            {
                if (Executor != null)
                {
                    result = Executor.Invoke(call);
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
                OnInvokeAfter?.Invoke(call, result, error);    
            }
            return result;
        }
    }

    public static class Proxy
    {
        public static (T Proxy, ProxyBean<T> Bean) Create<T>(
            Func<MethodCall, object?>? executor = null, 
            T? target = null) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new InvalidOperationException($"{typeof(T).Name} must be an interface");

            var proxy = DispatchProxy.Create<T, ProxyBean<T>>();
            var bean = (ProxyBean<T>)(object)proxy;
            bean.Target = target;
            bean.Executor = executor;
            return (proxy, bean);
        }
    
        public static (T Proxy, ProxyBean<T> Bean) Create<T>(T target)
            where T : class =>
            Create(null, target);
    }
}
