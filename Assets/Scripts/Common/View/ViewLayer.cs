using Common.Lang.Entity;
using Common.Lang.Observable;

namespace Common.View
{
    /// <summary>
    /// represents layer of viewManager
    /// </summary>
    public class ViewLayer : AbstractEntityIdString
    {
        public ViewManager Manager;
        
        /// <summary>
        /// active views
        /// </summary>
        public readonly ObsList<ViewInstance> Views = new ObsList<ViewInstance>();
        
        /// <summary>
        /// pending views (waiting to be shown exclusively on this layer)
        /// </summary>
        public readonly ObsList<ViewInstance> Pending = new ObsList<ViewInstance>();

        public override void Clear()
        {
            Pending.Clear();
            Views.Clear();
        }
    }
}