using Common.IO.Serialize.Newtonsoft.Json;
using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Common.Lang.Observable;
using Common.Util;
using Iso.Player;
using Newtonsoft.Json;

namespace Iso.Buildings
{
    public class BuildingsConverter : JsonConverterGeneric<PooledObsList<Building>>
    {
        private IsoWorld _world;
        
        public BuildingsConverter(IsoWorld world)
        {
            _world = world;
        }

        protected override void WriteJson(JsonWriter writer, PooledObsList<Building> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var building in value)
            {
                serializer.Serialize(writer, building);
            }
            writer.WriteEndArray();
        }

        protected override PooledObsList<Building>? ReadJson(JsonReader reader, PooledObsList<Building>? value, JsonSerializer serializer)
        {
            LangHelper.Validate(reader.IsStartArray());
            while (!reader.IsEndArray())
            {
                reader.Read();
                if (reader.IsStartObject())
                {
                    serializer.Deserialize<Building>(reader);
                }
            }

            return value;
        }
    }
}
