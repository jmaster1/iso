using System;
using Newtonsoft.Json;

namespace Common.IO.Serialize.Newtonsoft.Json.Converter
{
    /// <summary>
    /// serialize source object by converting to/from target type,
    /// generally used to serialize managed entity by its' info descriptor
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public class TransformConverter<TSource, TTarget> : JsonConverterGeneric<TSource>
    {
        private readonly Func<TSource, TTarget> transform;

        private readonly Func<TTarget, TSource> inverseTransform;

        public TransformConverter(Func<TSource, TTarget> transform, Func<TTarget, TSource> inverseTransform)
        {
            this.transform = transform;
            this.inverseTransform = inverseTransform;
        }

        protected override void WriteJson(JsonWriter writer, TSource value, JsonSerializer serializer)
        {
            var obj = transform(value);
            serializer.Serialize(writer, obj);
        }

        protected override TSource ReadJson(JsonReader reader, TSource value, JsonSerializer serializer)
        {
            var target = serializer.Deserialize<TTarget>(reader);
            return inverseTransform(target);
        }
    }
}