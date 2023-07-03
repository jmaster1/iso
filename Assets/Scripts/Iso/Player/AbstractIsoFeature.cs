using Common.Player;
using Iso.Player;

namespace Sample.Player
{
    public abstract class AbstractIsoFeature : AbstractFeature
    {
        public IsoPlayer Player => (IsoPlayer) AbstractPlayer;

        public PlayerInfo PlayerInfo => Player.PlayerInfo;
    }
}