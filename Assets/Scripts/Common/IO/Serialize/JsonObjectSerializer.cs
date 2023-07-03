using System;
using Common.IO.Streams;

namespace Common.IO.Serialize
{
    /// <summary>
    /// ObjectSerializer extension that uses Json
    /// </summary>
    public abstract class JsonObjectSerializer : AbstractObjectSerializer
    {
        public const string Format = "json";
        
        public override string GetFormat()
        {
            return Format;
        }

        public abstract string ToJson(object obj);

        public abstract object FromJson(string json, Type type);

        public T FromJson<T>(string json)
        {
            return (T) FromJson(json, typeof(T));
        }

        public override void Write(object obj, System.IO.Stream stream)
        {
            var json = ToJson(obj);
            stream.WriteString(json);
        }

        public override object Read(System.IO.Stream stream, Type type)
        {
            var json = stream.ReadString();
            return FromJson(json, type);
        }
    }
}
