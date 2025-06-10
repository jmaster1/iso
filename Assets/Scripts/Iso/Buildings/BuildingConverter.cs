using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Iso.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Iso.Buildings
{
    public class BuildingConverter : JsonConverterGeneric<Building>
    {
        private IsoWorld _world;
        
        public BuildingConverter(IsoWorld world)
        {
            _world = world;
        }
        
        protected override void WriteJson(JsonWriter writer, Building value, JsonSerializer serializer)
        {
            WriteObjectProperties(writer, 
                "id", value.Info.Id,
                "x", value.X, 
                "y", value.Y,
                "flipped", value.Flipped);
        }

        protected void WriteObjectProperties(JsonWriter writer, params object[] namesAndValues)
        {
            writer.WriteStartObject();
            for (var i = 0; i < namesAndValues.Length;)
            {
                writer.WritePropertyName((string)namesAndValues[i++]);
                writer.WriteValue(namesAndValues[i++]);    
            }
            writer.WriteEndObject();
        }

        protected override Building? ReadJson(JsonReader reader, Building? value, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var id = jo["id"].ToString();
            var x = jo["x"].ToObject<int>();
            var y = jo["y"].ToObject<int>();
            var flipped = jo["flipped"].ToObject<bool>();
            var info = _world.Buildings.BuildingInfoSet.GetById(id);
            return _world.Buildings.Build(info, x, y, flipped);
        }
    }
}
