using Common.Api.Info;
using Common.Lang;
using Common.Lang.Entity;
using Common.Util;
using Newtonsoft.Json;

namespace Common.IO.Serialize.Newtonsoft.Json.Converter
{
    /// <summary>
    /// AbstractEntityIdGenericConverter json converter,
    /// this writes entity id, requires InfoSetId for deserialization
    /// </summary>
    /// <typeparam name="T">type of entity</typeparam>
    /// <typeparam name="TKey">type of entity key</typeparam>
    public class AbstractEntityIdGenericConverter<T, TKey> : JsonConverterGeneric<T> 
        where T : AbstractEntityIdGeneric<TKey>
    {
        /// <summary>
        /// source InfoSet
        /// </summary>
        public InfoSetIdGeneric<T, TKey> InfoSet;

        /// <summary>
        /// this will be returned if not found on read (if not null), otherwise error will be thrown
        /// </summary>
        public T DefaultValue;

        public AbstractEntityIdGenericConverter(InfoSetIdGeneric<T, TKey> infoSet, T defaultValue = null)
        {
            InfoSet = infoSet;
            DefaultValue = defaultValue;
        }

        protected override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            var id = value == null ? (object) null : value.GetId();
            serializer.Serialize(writer, id);
        }

        protected override T ReadJson(JsonReader reader, T? value, JsonSerializer serializer)
        {
            var id = serializer.Deserialize<TKey>(reader);
            var info = InfoSet.FindById(id);
            if (info != null) return info;
            if (DefaultValue != null)
            {
                info = DefaultValue;
            }
            else
            {
                LangHelper.Throw($"{typeof(T)}.{id} not found");
            }
            return info;
        }
    }
}