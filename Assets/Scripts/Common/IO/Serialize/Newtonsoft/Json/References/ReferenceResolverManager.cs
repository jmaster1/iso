using System;
using Common.Lang;
using Common.Lang.Entity;
using Common.Util;
using Newtonsoft.Json.Serialization;

namespace Common.IO.Serialize.Newtonsoft.Json.References
{
    /// <summary>
    /// IReferenceResolver implementation that uses registry of IReferenceConverter
    /// to handle json references
    /// </summary>
    public class ReferenceResolverManager : GenericBean, IReferenceResolver
    {
        /// <summary>
        /// used to separate prefix from reference
        /// </summary>
        private const char Delimiter = ':';
        
        Map<Type, IReferenceConverter> convertersByType = new Map<Type, IReferenceConverter>();
        
        Map<string, IReferenceConverter> convertersByPrefix = new Map<string, IReferenceConverter>();

        /// <summary>
        /// this should be used to register arbitrary converters
        /// </summary>
        public void AddConverter(IReferenceConverter converter)
        {
            converter.RegisterTypes(type =>
            {
                convertersByType[type] = converter;
            });
            var prefix = converter.GetPrefix();
            convertersByPrefix[prefix] = converter;
        }
        
        public object ResolveReference(object context, string reference)
        {
            if (reference == null) return null;
            var index = reference.IndexOf(Delimiter);
            var prefix = reference.Substring(0, index);
            var converter = convertersByPrefix.Get(prefix);
            var suffix = reference.Substring(index + 1);
            var value = converter.FromReference(suffix);
            return value;
        }

        public string GetReference(object context, object value)
        {
            if (value == null) return null;
            var type = value.GetType();
            var converter = convertersByType.Find(type);
            LangHelper.Validate(converter != null, "No converter for type: {0}", type);
            var reference = converter.GetPrefix() + Delimiter + converter.ToReference(value);
            return reference;
        }

        public bool IsReferenced(object context, object value)
        {
            return true;
        }

        public void AddReference(object context, string reference, object value)
        {
        }
    }
}
