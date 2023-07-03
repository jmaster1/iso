using System;
using Common.Lang.Entity;

namespace Common.Lang.Observable
{
    /// <summary>
    /// non-generic holder
    /// </summary>
    public abstract class HolderRaw : AbstractEntityIdString
    {
        public abstract object GetRaw();
        
        public abstract void SetRaw(object val);

        public abstract Type GetValueType();
    }
}