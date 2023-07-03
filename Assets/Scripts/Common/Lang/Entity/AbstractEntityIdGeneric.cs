namespace Common.Lang.Entity
{
    /// <summary>
    /// AbstractEntity extension with generic id 
    /// </summary>
    public abstract class AbstractEntityIdGeneric<TID> : AbstractEntity, IIdAware<TID>
    {
        /// <summary>
        /// property name for "name"
        /// </summary>
        public const string PropertyName = "Name";
        
        /// <summary>
        /// entity identifier
        /// </summary>
        public TID Id;

        /// <summary>
        /// localized name retrieval, the key for value is {Type.Name}.{id}.Name
        /// </summary>
        public virtual string GetLocalName()
        {
            return GetPropertyMessage(PropertyName);
        }

        /// <summary>
        /// retrieve localized message for named property of this bean using pattern
        /// {obj.type.name}.{obj.id}.{propertyName}
        /// </summary>
        public string GetPropertyMessage(string propertyName)
        {
            return LocalApi.GetObjectMessage(this, GetId().ToString(), propertyName);
        }
        
        public virtual TID GetId()
        {
            return Id;
        }

        public override void Clear()
        {
            Id = default;
            base.Clear();
        }
    }
}
