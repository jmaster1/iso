using System.Reflection;
using System.Text;

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

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(MethodInfo.DeclaringType?.Name)
            .Append('.')
            .Append(MethodInfo.Name)
            .Append('(');
        if (Args is { Length: > 0 })
        {
            for (var i = 0; i < Args.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(Args[i]);                
            }
        }
        sb.Append(')');
        return sb.ToString();
    }
}
