using Common.Api.Info;
using Common.Lang;
using Common.Lang.Entity;

namespace Common.IO.Serialize.Newtonsoft.Json.Converter
{
    public class AbstractEntityIdStringConverter<T> : AbstractEntityIdGenericConverter<T, string>
        where T : AbstractEntityIdString
    {
        public AbstractEntityIdStringConverter(InfoSetIdGeneric<T, string> infoSet, 
            T defaultValue = default) : base(infoSet, defaultValue)
        {
        }
    }
}