using Common.IO.Serialize.Newtonsoft.Json;
using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Iso.Player;
using Newtonsoft.Json;

namespace Iso.Cells
{
    public class CellConverter : JsonConverterGeneric<Cell>
    {
        private IsoWorld _world;
        
        public CellConverter(IsoWorld world)
        {
            _world = world;
        }

        protected override void WriteJson(JsonWriter writer, Cell value, JsonSerializer serializer)
        {
            WriteObjectAndProperties(writer, 
                "x", value.X, 
                "y", value.Y);
        }

        protected override Cell? ReadJson(JsonReader reader, Cell? value, JsonSerializer serializer)
        {
            var jo = reader.ReadJObject();
            var x = jo.To<int>("x");
            var y = jo.To<int>("y");
            return _world.Cells.Find(x, y);
        }
    }
}
