using System;
using Common.Lang.Observable;
using Common.Player;

namespace Iso.Player
{
    public abstract class AbstractIsoFeature<TEvent, TEntity> : AbstractFeature where TEvent : Enum
    {
        public IsoWorld World => (IsoWorld) AbstractPlayer;

        //public PlayerInfo PlayerInfo => Player.PlayerInfo;
        
        public Events<TEvent, TEntity?> Events = new();
        
        internal void FireEvent(TEvent type, TEntity? entity = default)
        {
            Events.Fire(type, entity);
        }
    }
}
