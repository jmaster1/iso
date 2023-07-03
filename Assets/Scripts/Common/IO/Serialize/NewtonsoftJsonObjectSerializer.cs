using System;
using Newtonsoft.Json;

namespace Common.IO.Serialize
{
    /// <summary>
    /// JsonObjectSerializer extension that uses Newtonsoft.Json
    /// </summary>
    public class NewtonsoftJsonObjectSerializer : JsonObjectSerializer
    {
        public static readonly NewtonsoftJsonObjectSerializer Instance = new NewtonsoftJsonObjectSerializer();
        
        public Formatting Formatting = Formatting.Indented;

        public override string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting);
        }

        public override object FromJson(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
