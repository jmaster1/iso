using System.IO;
using Common.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.IO.Serialize.Newtonsoft.Json
{
    public static class NewtonsoftJsonExtensions
    {    
        public static string ToJson(this JsonSerializer serializer, object value)
        {
            var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, value);
            return stringWriter.ToString();
        }

        public static T FromJson<T>(this JsonSerializer serializer, string json) => 
            (T) serializer.Deserialize(new StringReader(json), typeof(T))!;

        public static void Populate(this JsonSerializer serializer, object value, string json)
        {
            TextReader reader = new StringReader(json);
            serializer.Populate(reader, value);
        }
        
        public static void Copy(this JsonSerializer serializer, object source, object target)
        {
            string json = serializer.ToJson(source);
            serializer.Populate(target, json);
        }
    
        public static bool IsNull(this JsonReader reader)
        {
            return reader.TokenType == JsonToken.Null;
        }
        
        public static bool IsPropertyName(this JsonReader reader)
        {
            return reader.TokenType == JsonToken.PropertyName;
        }
        
        public static bool IsStartObject(this JsonReader reader)
        {
            return reader.TokenType == JsonToken.StartObject;
        }
        
        public static bool IsEndObject(this JsonReader reader)
        {
            return reader.TokenType == JsonToken.EndObject;
        }
        
        public static bool IsStartArray(this JsonReader reader)
        {
            return reader.TokenType == JsonToken.StartArray;
        }
        
        public static bool IsEndArray(this JsonReader reader)
        {
            return reader.TokenType == JsonToken.EndArray;
        }
        
        public static bool IsString(this JsonReader reader)
        {
            return reader.TokenType == JsonToken.String;
        }

        public static string ReadPropertyName(this JsonReader reader)
        {
            reader.Read();
            LangHelper.Validate(reader.IsPropertyName());
            return (string) reader.Value!;
        }
        
        public static void ReadStartObject(this JsonReader reader)
        {
            reader.Read();
            LangHelper.Validate(reader.IsStartObject());
        }
        
        public static void ReadEndObject(this JsonReader reader)
        {
            reader.Read();
            LangHelper.Validate(reader.IsEndObject());
        }
        
        public static JObject ReadJObject(this JsonReader reader) => 
            JObject.Load(reader);
        
        public static T To<T>(this JObject jobj, string name) => 
            jobj[name]!.ToObject<T>()!;
    }
}
