using System;
using Common.Lang.Entity;
using Common.Lang.Observable;

namespace Iso.Util
{

    public class AbstractManager<TEvent, TEntity> : GenericBean where TEvent : Enum
    {
        public Events<TEvent, TEntity> Events = new();
        
        internal void FireEvent(TEvent type, TEntity entity)
        {
            Events.Fire(type, entity);
        }
    }

}