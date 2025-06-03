using Common.Lang;

namespace IsoNet.Core.Transport.Rmi.Json;

public class FastTypeStringConverter : IConverter<Type, string>
{
    private readonly Dictionary<Type, string> _forward = new();
    private readonly Dictionary<string, Type> _reverse = new();

    public FastTypeStringConverter(params Type[] types)
    {
        foreach (var type in types)
        {
            RegisterType(type);
        }
    }
    
    public void RegisterType(Type type)
    {
        _forward[type] = type.Name;
        _reverse[type.Name] = type;
    }
    
    public string Convert(Type source) => _forward[source];

    public Type Revert(string target) => _reverse[target];
}
