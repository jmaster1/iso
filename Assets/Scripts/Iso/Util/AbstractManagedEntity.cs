using System;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Iso.Player;
using Newtonsoft.Json;

namespace Iso.Util
{

    public class AbstractManagedEntity<TManager, TEvent, TEntity> : AbstractEntity 
        where TManager : AbstractIsoFeature<TEvent, TEntity>
        where TEvent : Enum
        where TEntity : AbstractManagedEntity<TManager, TEvent, TEntity>
    {
        [JsonIgnore]
        public TManager Manager;
        
        [JsonIgnore]
        public Events<TEvent, TEntity> Events => Manager.Events;
        
        protected void FireEvent(TEvent evt)
        {
            Manager.FireEvent(evt, (TEntity)this);
        }
    }
    

}