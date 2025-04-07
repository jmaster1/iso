using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Api.Input;
using Common.ContextNS;
using Common.IO.Streams;
using Common.Lang.Collections;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.Util;
using Common.Util.Http;

namespace Common.View
{
    /// <summary>
    /// a model for managing ui presence on screen (singleton)
    /// </summary>
    public class ViewManager : GenericBean
    {
        /// <summary>
        /// layer name for debug components
        /// </summary>
        public const string LayerDebug = "Debug";

        /// <summary>
        /// we need to lock input while showing/hiding views
        /// </summary>
        public readonly InputApi InputApi = Context.Get<InputApi>();

        public ObsListMapString<ViewLayer> Layers = new();

        private static readonly Map<Type, string> TypeToViewId = new();
        
        /// <summary>
        /// resolve view id from its' type
        /// </summary>
        public static string GetViewId(Type viewType)
        {
            var viewId = TypeToViewId.Find(viewType);
            if (viewId != null) return viewId;
            var ns = viewType.Namespace;
            var index = ns!.IndexOf("UI.", StringComparison.InvariantCulture);
            viewId = TypeToViewId[viewType] = ns[index..].Replace('.', '/') + "/" + viewType.Name;
            return viewId;
        }

        public static string GetViewId<T>()
        {
            return GetViewId(typeof(T));
        }

        /// <summary>
        /// add new layer
        /// </summary>
        /// <param name="layerId">layer unique id</param>
        /// <returns></returns>
        public ViewLayer AddLayer(string layerId)
        {
            var layer = new ViewLayer()
            {
                Id = layerId,
                Manager = this
            };
            Layers.Add(layer);
            return layer;
        }
        
        /// <summary>
        /// add layers using enum constants
        /// </summary>
        public void AddLayers<T>() where T : Enum
        {
            var values = LangHelper.EnumValues<T>();
            foreach (var e in values)
            {
                AddLayer(e.ToString());
            }
        }
        
        /// <summary>
        /// animate (or not) given ui, lock input while animation, eventually invoke callback
        /// </summary>
        /// <param name="viewInstance"></param>
        /// <param name="animate">shows whether show/hide should be animated</param>
        /// <param name="shows">show phase (true) or hide (false)</param>
        /// <param name="callback"></param>
        private void Animate(ViewInstance viewInstance, bool animate, bool show, Action callback)
        {
            var view = viewInstance.View;
            if (!animate)
            {
                callback?.Invoke();
            }
            else
            {
                InputApi.InputLockAdd(viewInstance);
                var callbackWrapper = new Action(() =>
                {
                    InputApi.InputLockRemove(viewInstance);
                    callback?.Invoke();
                });
                var processed = view.PlayAnimation(show, callbackWrapper);
                if (!processed)
                {
                    callbackWrapper();
                }
            }
        }
        
        private void UpdateState(ViewInstance viewInstance, ViewState state)
        {
            if(Log.IsDebugEnabled) Log.DebugFormat("view={0}, state={1}", viewInstance.ViewId, state);
            viewInstance.State = state;
        }
        
        internal ViewInstance CreateViewInstance()
        {
            var e = new ViewInstance
            {
                Manager = this
            };
            return e;
        }

        /// <summary>
        /// find first view matching criteria (non-null args)
        /// </summary>
        public ViewInstance Find(string viewId, string layerId = null, 
            object model = null, ViewState state = ViewState.Undefined)
        {
            foreach (var layer in Layers)
            {
                if (layerId != null && !StringHelper.Equals(layerId, layer.Id))
                {
                    continue;
                }

                var result = Find(viewId, model, state, layer.Views);
                if (result != null) return result;
                result = Find(viewId, model, state, layer.Pending);
                if (result != null) return result;
            }

            return null;
        }

        private static ViewInstance Find(string viewId, object model, 
            ViewState state, IEnumerable<ViewInstance> views)
        {
            return views.FirstOrDefault(view => view.Matches(viewId, model, state));
        }

        /// <summary>
        /// create ViewInstance, use chained setters and eventually call Show()
        /// </summary>
        /// <param name="viewId">view identifier</param>
        /// <param name="model">model to bind view to</param>
        public ViewInstance Create(string viewId, object model = null)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("Show: viewId={0}", viewId);
            var e = CreateViewInstance();
            e.ViewId = viewId;
            e.Model = model;
            return e;
        }

        public ViewInstance Create<T>()
        {
            var viewId = GetViewId<T>();
            return Create(viewId);
        }

        internal Task Show(ViewInstance viewInstance, bool animate = true, Action onComplete = null)
        {
            Validate(viewInstance.ViewId != null);
            var layer = viewInstance.Layer;
            Validate(layer != null);
            var promise = viewInstance.ShowPromise = new TaskCompletionSource<object>();
            viewInstance.AnimateShow = animate;
            viewInstance.AnimationShowCallback = onComplete;
            //
            // check if pending
            if (viewInstance.Exclusive && layer.Views.Count > 0)
            {
                viewInstance.State = ViewState.Pending;
                layer.Pending.Add(viewInstance);
            }
            else
            {
                viewInstance.State = ViewState.Showing; 
                layer.Views.Add(viewInstance);
            }

            return promise.Task;
        }
        
        internal void ViewAttached(ViewInstance viewInstance, IView view)
        {
            viewInstance.View = view;
            view.ViewInstance = viewInstance;
            Animate(viewInstance, viewInstance.AnimateShow, true, () =>
            {
                UpdateState(viewInstance, ViewState.Shown);
                viewInstance.ShowPromise.SetResult(viewInstance);
                viewInstance.AnimationShowCallback?.Invoke();
            });
        }

        /// <summary>
        /// hide view with given id
        /// </summary>
        internal Task Hide(ViewInstance viewInstance, 
            bool animate = true, Action onComplete = null)
        {
            var promise = new TaskCompletionSource<object>();
            LangHelper.Validate(viewInstance != null);
            UpdateState(viewInstance, ViewState.Hiding);
            Animate(viewInstance, animate, false, () =>
            {
                UpdateState(viewInstance, ViewState.Hidden);
                onComplete?.Invoke();
                var layer = viewInstance.Layer;
                layer.Views.Remove(viewInstance);
                promise.SetResult(null);
                //
                // check pending
                if(layer.Pending.Count == 0) return;
                var pending = layer.Pending.RemoveAtGet(0);
                Show(pending, pending.AnimateShow, pending.AnimationShowCallback);
            });
            return promise.Task;
        }
        
        /// <summary>
        /// hide all the views
        /// </summary>
        public void HideAll()
        {
            foreach (var layer in Layers)
            {
                layer.Clear();
            }
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            void RenderViews(IEnumerable<ViewInstance> views)
            {
                
                foreach (var e in views)
                {
                    html.tr().tdRowNum()
                        .td(e.ViewId)
                        .td(e.State)
                        .td(e.Model?.GetType().Name)
                        .td(e.Layer.Id)
                        .td(e.Exclusive)
                        .td().renderInvokeMethods(e, this).endTd()
                        .endTr();
                }
            }

            html.tableHeader("#", "viewId", "state", "model", "layer", "exclusive", "actions");
            foreach (var layer in Layers)
            {
                RenderViews(layer.Views);
                RenderViews(layer.Pending);    
            }
            html.endTable();
        }
    }
}