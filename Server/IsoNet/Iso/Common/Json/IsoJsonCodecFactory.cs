using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Iso.Cells;
using Iso.Player;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using Newtonsoft.Json;

namespace IsoNet.Iso.Common.Json;

public class IsoJsonCodecFactory
{
    public static IList<JsonConverter> AddConverters(IsoPlayer player, IList<JsonConverter>? list = null)
    {
        list ??= new List<JsonConverter>();
        list.Add(new CellConverter(player.Cells));
        return list;
    }

    public static JsonCodec<MethodCall> CreateCodec(IsoPlayer player)
    {
        return MethodCallJsonConverter.CreateCodec(settings =>
        {
            AddConverters(player, settings.Converters);
        });
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
