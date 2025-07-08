using System;
using Common.Lang;

namespace Common.IO.Transport.Rmi.Json
{
    public class TypeStringConverter : IConverter<Type, string>
    {
        public static readonly TypeStringConverter Instance = new();
    
        public string Convert(Type source) => source.AssemblyQualifiedName!;

        public Type Revert(string target) => Type.GetType(target)!;
    }
}
