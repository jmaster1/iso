using System.IO;
using Common.Util;
using Newtonsoft.Json;

namespace Common.IO.Serialize.Newtonsoft.Json
{
    public static class NewtonsoftJsonExtensions
    {    
        public static string ToJson(this JsonSerializer serializer, object value)
        {
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, value);
            string json = stringWriter.ToString();
            return json;
        }

        public static T FromJson<T>(this JsonSerializer serializer, string json)
        {
            T value = (T) serializer.Deserialize(new StringReader(json), typeof(T));
            return value;
        }
        
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
            string ret = (string) reader.Value;
            return ret;
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
    }
}
