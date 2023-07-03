using System.Collections.Generic;
using Common.ContextNS;
using Common.Player;

namespace Iso.Player
{
    /// <summary>
    /// AbstractPlayer extension for iso game
    /// </summary>
    public class IsoPlayer : AbstractPlayer
    {
        public readonly PlayerInfo PlayerInfo = Context.GetInfo<PlayerInfo>();
            

        protected override IEnumerable<AbstractFeature> GetFeatures()
        {
            //
            // the order is important
            return new AbstractFeature[]
            {
            
            };
        }
    }
}