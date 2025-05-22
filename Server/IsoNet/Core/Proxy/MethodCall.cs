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
        
        var parameters = MethodInfo.GetParameters();
        if (parameters.Length > 0)
        {
            for(var i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                var param = parameters[i];
                var argValue = Args != null && i < Args.Length ? Args[i] : "null";

                sb.Append(param.ParameterType.Name)
                    .Append(' ')
                    .Append(param.Name)
                    .Append('=')
                    .Append(argValue ?? "null");
            }
        }
        
        sb.Append(')');
        return sb.ToString();
    }
}
