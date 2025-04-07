using Iso.Player;

namespace Iso.Tests
{
    public class AbstractPlayerTest
    {
        public readonly IsoPlayer Player = new();
        public Cells.Cells Cells => Player.Cells;
        public Buildings.Buildings Buildings => Player.Buildings;
        public Movables.Movables Movables => Player.Movables;
    }
}
