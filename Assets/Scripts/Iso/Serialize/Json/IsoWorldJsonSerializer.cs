using Iso.Player;

namespace Iso.Serialize.Json
{
    public class IsoWorldJsonSerializer : AbstractPlayerJsonSerializer<IsoWorld>
    {
        public IsoWorldJsonSerializer(IsoWorld player) : base(player)
        {
        }
    }
}