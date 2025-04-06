using System;
using Common.Lang.Entity;
using Common.Lang.Observable;

namespace Iso.Util
{

    public class AbstractManagedEntity<TManager, TEvent, TEntity> : AbstractEntity 
        where TManager : AbstractManager<TEvent, TEntity>
        where TEvent : Enum
        where TEntity : AbstractManagedEntity<TManager, TEvent, TEntity>
    {
        public TManager Manager;
        
        public Events<TEvent, TEntity> Events => Manager.Events;
        
        protected void FireEvent(TEvent evt)
        {
            Manager.FireEvent(evt, (TEntity)this);
        }
    }
    

}