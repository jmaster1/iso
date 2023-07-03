using System;

namespace Common.IO.Serialize.Newtonsoft.Json.References
{
    /// <summary>
    /// api for transforming object into string references and back,
    /// result reference formatted as {prefix}:{suffix}
    /// </summary>
    public interface IReferenceConverter
    {
        /// <summary>
        /// unique prefix retrieval that used as 
        /// </summary>
        string GetPrefix();
        
        /// <summary>
        /// invoked upon converter registration in order to register ALL the supported types of convertible objects
        /// </summary>
        void RegisterTypes(Action<Type> callback);
        
        /// <summary>
        /// subclasses should implement suffix conversion to object 
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        object FromReference(string reference);

        /// <summary>
        /// subclasses should implement object conversion to suffix
        /// </summary>
        string ToReference(object value);
    }
}
