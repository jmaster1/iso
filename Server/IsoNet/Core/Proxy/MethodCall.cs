using System.Reflection;

namespace IsoNet.Core.Proxy;

public class MethodCall
{
    public MethodInfo MethodInfo = null!;
    
    public object?[]? Args;

    public Dictionary<string, object>? Attrs;
    
    public Func<string, Type, object?, object?>? AttrGetter;

    public void SetAttr(string name, int timeFrame)
    {
        Attrs ??= new Dictionary<string, object>();
        Attrs[name] = timeFrame;
    }

    public T? GetAttr<T>(string name, T defaultValue = default!)
    {
        return (T?)(AttrGetter == null ? Attrs?[name] : AttrGetter(name, typeof(T), defaultValue));
    }
}
