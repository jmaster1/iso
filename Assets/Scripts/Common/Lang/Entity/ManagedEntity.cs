namespace Common.Lang.Entity
{
    /// <summary>
    /// entity managed by some manager
    /// </summary>
    public class ManagedEntity<T> : AbstractEntityIdString
    {
        public T Manager { get; set; }
        
        public override void Clear()
        {
            Manager = default;
            base.Clear();
        }
    }
}