using System;

namespace Common.Lang.Entity
{
    /// <summary>
    /// AbstractIdGenericEntity extension with enum id 
    /// </summary>
    public abstract class AbstractEntityIdEnum<T> : AbstractEntityIdGeneric<T> where T : Enum
    {
    }
}