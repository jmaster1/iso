using Common.IO.Serialize.Newtonsoft.Json;
using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Iso.Player;
using Newtonsoft.Json;

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
            WriteObjectAndProperties(writer, 
                "id", value.Info.Id,
                "x", value.X, 
                "y", value.Y,
                "flipped", value.Flipped);
        }
        
        protected override Building? ReadJson(JsonReader reader, Building? value, JsonSerializer serializer)
        {
            
            var jo = reader.ReadJObject();
            var id = jo.To<string>("id");
            var x = jo.To<int>("x");
            var y = jo.To<int>("y");
            var flipped = jo.To<bool>("flipped");
            var info = _world.Buildings.BuildingInfoSet.GetById(id);
            return _world.Buildings.Build(info, x, y, flipped);
        }
    }
}
