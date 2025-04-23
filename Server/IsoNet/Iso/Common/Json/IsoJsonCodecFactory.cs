using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Iso.Cells;
using Newtonsoft.Json;

namespace IsoNet.Iso.Common.Json;

public class IsoJsonCodecFactory
{
    
}

public class CellConverter(Cells cells) : JsonConverterGeneric<Cell>
{
    private const int MultiplierX = 10000;
    
    protected override void WriteJson(JsonWriter writer, Cell value, JsonSerializer serializer)
    {
        var v = value.X * MultiplierX + value.Y;
        serializer.Serialize(writer, v);
    }

    protected override Cell ReadJson(JsonReader reader, Cell value, JsonSerializer serializer)
    {
        var v = reader.ReadAsInt32();
        var x = v / MultiplierX;
        var y = v % MultiplierX;
        return cells.Find((int)x, (int)y);
    }
}
