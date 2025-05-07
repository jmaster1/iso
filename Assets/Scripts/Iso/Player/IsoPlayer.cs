using System.Collections.Generic;
using Common.Player;

namespace Iso.Player
{
    /// <summary>
    /// AbstractPlayer extension for iso game
    /// </summary>
    public class IsoPlayer : AbstractPlayer
    {
        //public readonly PlayerInfo PlayerInfo = Context.GetInfo<PlayerInfo>();
        public readonly string Guid;

        public readonly Cells.Cells Cells = new();
        
        public readonly Buildings.Buildings Buildings = new();
        
        public readonly Movables.Movables Movables = new();

        public IsoPlayer(string guid = null!)
        {
            Guid = guid;
        }

        protected override IEnumerable<AbstractFeature> InitFeatures()
        {
            //
            // the order is important
            return new AbstractFeature[]
            {
                Cells,
                Buildings,
                Movables
            };
        }
    }
}