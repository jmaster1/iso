using System.IO;
using Common.IO.Serialize.Newtonsoft.Json;
using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Common.IO.Streams;
using Common.Lang.Observable;
using Common.Util;
using Iso.Player;
using Newtonsoft.Json;

namespace Iso.Cells
{
    public class CellsConverter : JsonConverterGeneric<PooledObsList<Cell>>
    {
        private IsoWorld _world;
        public CellsConverter(IsoWorld world)
        {
            _world = world;
        }

        //Map<ResourceType, long> map = new Map<ResourceType, long>();

        protected override void WriteJson(JsonWriter writer, PooledObsList<Cell> value, JsonSerializer serializer)
        {
            var ms = new MemoryStream();
            var data = ms.GZip().DataWriter();
            var cells = _world.Cells;
            var list = cells.CellList;
            
            data.Write(cells.Width);
            data.Write(cells.Heigth);
            data.Write(list.Count);
            foreach (var cell in list)
            {
                data.Write(cell.x);
                data.Write(cell.y);
                data.Write((int)cell.CellType);
            }

            writer.WriteStartObject();
            writer.WritePropertyName("cells");
            writer.WriteValue(ms.ToBase64String());
            writer.WriteEndObject();
        }

        protected override PooledObsList<Cell>? ReadJson(JsonReader reader, PooledObsList<Cell>? value, JsonSerializer serializer)
        {
            if (reader.IsNull()) return value;
            LangHelper.Validate(reader.IsStartObject());
            
            reader.Read();
            while (reader.IsPropertyName())
            {
                var name = (string)reader.Value!;
                if ("cells".Equals(name))
                {
                    var cells = _world.Cells;
                    var list = cells.CellList;
                    var b64 = reader.ReadAsString()!;
                    var data = b64.FromBase64StringStream().GUnZip().DataReader();
                    var w = cells.Width = data.ReadInt32();
                    var h = cells.Heigth = data.ReadInt32();
                    var cellCount = data.ReadInt32();
                    for (var i = 0; i < cellCount; i++)
                    {
                        var x = data.ReadInt32();
                        var y = data.ReadInt32();
                        var cellType = data.ReadInt32();
                        cells.Set(x, y, (CellType)cellType);
                    }

                } else reader.Skip();
                reader.Read();
            }
            LangHelper.Validate(reader.IsEndObject());
            return value;
        }
    }
}