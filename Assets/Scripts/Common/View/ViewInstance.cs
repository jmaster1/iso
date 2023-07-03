using System;
using System.Threading.Tasks;
using Common.Util;
using Newtonsoft.Json;

namespace Common.View
{
    /// <summary>
    /// represents view instance that should be present on screen
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ViewInstance
    {
        /// <summary>
        /// parent manager
        /// </summary>
        public ViewManager Manager { get; internal set; }
        
        /// <summary>
        /// view identifier, this should be possible to resolve actual view component at view module
        /// </summary>
        [JsonProperty]
        public string ViewId { get; internal set; }

        /// <summary>
        /// model for view binding
        /// </summary>
        [JsonProperty(IsReference = true)]
        public object Model { get; internal set; }

        /// <summary>
        /// layer where view should be added (used as global z-index)
        /// </summary>
        [JsonProperty]
        public ViewLayer Layer { get; internal set; }

        /// <summary>
        /// shows whether requested to show exclusively for the layer
        /// </summary>
        [JsonProperty]
        public bool Exclusive { get; internal set; }
        
        /// <summary>
        /// shows whether view should be kept alive (bound) when hidden
        /// </summary>
        public bool KeepAlive { get; internal set; }

        /// <summary>
        /// current state of view
        /// </summary>
        public ViewState State = ViewState.Hidden;

        /// <summary>
        /// show view should be animated
        /// </summary>
        public bool AnimateShow;
        
        /// <summary>
        /// callback to invoke after show animation
        /// </summary>
        public Action AnimationShowCallback;
        
        /// <summary>
        /// attached view object
        /// </summary>
        public IView View;

        public TaskCompletionSource<object> ShowPromise;

        public override string ToString()
        {
            return ViewId;
        }

        public Task Hide(bool animate = true, Action onComplete = null)
        {
            return Manager.Hide(this, animate, onComplete);
        }
        
        public Task Show(bool animate = true, Action onComplete = null)
        {
            return Manager.Show(this, animate, onComplete);
        }

        /// <summary>
        /// should be called by ui manager when view added to screen
        /// and should be attached to instance
        /// </summary>
        public void ViewAttached(IView view)
        {
            Manager.ViewAttached(this, view);
        }
        
        /// <summary>
        /// should be called by ui manager when view removed from screen
        /// and should be detached from instance
        /// </summary>
        public void ViewDetached()
        {
            View = null;
        }

        public ViewInstance SetModel(object model)
        {
            Model = model;
            return this;
        }

        public ViewInstance SetLayer(ViewLayer layer)
        {
            Layer = layer;
            return this;
        }
        
        public ViewInstance SetLayer(string layerId)
        {
            var layer = Manager.Layers.GetByKey(layerId);
            return SetLayer(layer);
        }
        
        public ViewInstance SetLayer(Enum layerEnum)
        {
            var layerId = layerEnum.ToString();
            return SetLayer(layerId);
        }
        
        public ViewInstance SetLayerDebug()
        {
            return SetLayer(ViewManager.LayerDebug);
        }
        
        public ViewInstance SetExclusive(bool exclusive = true)
        {
            Exclusive = exclusive;
            return this;
        }

        public ViewInstance SetKeepAlive()
        {
            KeepAlive = true;
            return this;
        }

        public bool Matches(string viewId, object model, ViewState state)
        {
            return (viewId == null || StringHelper.Equals(viewId, ViewId))
                   && (model == null || model == Model)
                   && (state == ViewState.Undefined || state == State);
        }
    }
}
