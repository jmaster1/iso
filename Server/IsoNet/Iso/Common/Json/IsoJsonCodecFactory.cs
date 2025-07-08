using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Iso.Cells;
using Iso.Player;
using Common.IO.Codec;
using IsoNet.Core.Transport.Rmi.Json;
using Newtonsoft.Json;

namespace IsoNet.Iso.Common.Json;

public static class IsoJsonCodecFactory
{
    public static JsonCodec CreateCodec()
    {
        return TransportRmiJsonCodecFactory.CreateCodec(
            typeof(IIsoClientApi), 
            typeof(IIsoServerApi),
            typeof(IIsoWorldApi)
        );
    }

    public static JsonCodec AddWorldConverters(JsonCodec codec, IsoWorld world)
    {
        codec.AddConverter(new CellConverter(world.Cells));
        return codec;
    }
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
        var v = serializer.Deserialize<int>(reader);
        var x = v / MultiplierX;
        var y = v % MultiplierX;
        return cells.Get(x, y);
    }
}
