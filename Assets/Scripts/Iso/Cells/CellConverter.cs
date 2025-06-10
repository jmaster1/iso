using Common.IO.Serialize.Newtonsoft.Json;
using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Common.Util;
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
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        protected override Cell? ReadJson(JsonReader reader, Cell? value, JsonSerializer serializer)
        {
            LangHelper.Validate(reader.IsStartObject());
            reader.Read();
            int x = 0, y = 0;
            while (reader.IsPropertyName())
            {
                var name = (string) reader.Value;
                if ("x".Equals(name))
                {
                    x = (int)reader.ReadAsInt32();
                } else if ("y".Equals(name))
                {
                    y = (int)reader.ReadAsInt32();
                } else reader.Skip();
                reader.Read();
            }
            LangHelper.Validate(reader.IsEndObject());
            return _world.Cells.Find(x, y);
        }
    }
}
