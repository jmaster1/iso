using System;
using Common.Lang.Observable;
using Common.Player;

namespace Iso.Player
{
    public abstract class AbstractIsoFeature<TEvent, TEntity> : AbstractFeature where TEvent : Enum
    {
        public IsoPlayer Player => (IsoPlayer) AbstractPlayer;

        //public PlayerInfo PlayerInfo => Player.PlayerInfo;
        
        public Events<TEvent, TEntity> Events = new();
        
        internal void FireEvent(TEvent type, TEntity entity)
        {
            Events.Fire(type, entity);
        }
    }
}
