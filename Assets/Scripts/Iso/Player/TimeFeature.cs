using Common.Player;
using Common.TimeNS;
using Newtonsoft.Json;

namespace Iso.Player
{
    public class TimeFeature : AbstractFeature
    {
        [JsonProperty]
        public Time TimeGame => AbstractPlayer.TimeGame;
    }
}
