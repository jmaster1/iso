using System;
using System.Collections.Generic;
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
        
        protected void SetProperty<T>(ref T field, T value, TEvent eventType)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            FireEvent(eventType);
        }
    }
}