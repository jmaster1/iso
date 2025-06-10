using Iso.Cells;
using Iso.Player;
using Newtonsoft.Json;

namespace Iso.Serialize.Json
{
    public class IsoWorldJsonSerializer : AbstractPlayerJsonSerializer<IsoWorld>
    {
        public IsoWorldJsonSerializer(IsoWorld player) : base(player)
        {
        }

        protected override void DecorateSettings(JsonSerializerSettings settings)
        {
            settings.Converters.Add(new CellsConverter(player));
        }
    }
}