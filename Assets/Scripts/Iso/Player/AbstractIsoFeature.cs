using Common.Player;

namespace Iso.Player
{
    public abstract class AbstractIsoFeature : AbstractFeature
    {
        public IsoPlayer Player => (IsoPlayer) AbstractPlayer;

        public PlayerInfo PlayerInfo => Player.PlayerInfo;
    }
}