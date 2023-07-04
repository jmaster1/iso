using Common.Lang.Observable;
using Common.Unity.Bind;
using Common.View;
using UnityEngine;

namespace Common.Unity.View
{
    /// <summary>
    /// unity adapter for ViewManager,
    /// responsible for instantiating game object for each layer
    /// and keeping views in sync with ViewInstances  
    /// </summary>
    public class UnityViewManagerAdapter : BindableMono<ViewManager> 
    {
        /// <summary>
        /// holds layers for objects
        /// </summary>
        public Transform Root => transform;

        /// <summary>
        /// predefined layers to use (match gameObject name to layer id)
        /// </summary>
        public GameObject[] predefinedLayers;

        public readonly ObsListMap<ViewLayer, UnityViewLayer> Layers = 
            new ObsListMap<ViewLayer, UnityViewLayer>();
       
        private ObsListAdapter<ViewLayer, UnityViewLayer> layerAdapter;
        
        public override void OnBind()
        {
            layerAdapter ??= new ObsListAdapter<ViewLayer, UnityViewLayer>
            {
                Views = Layers,
                CreateView = CreateUnityViewLayer
            };
            BindBindable(Model.Layers, layerAdapter);
            BindToHolder(LocalApi.Language, OnLanguageChange, false);
        }

        /// <summary>
        /// localise and rebind (to update dynamic labels) all views on all layers
        /// and pooled views
        /// </summary>
        private void OnLanguageChange()
        {
            var localiser = ViewPoolApi.Localiser;
            if(localiser == null) return;
            foreach (var layer in Layers)
            {
                foreach (var view in layer.viewAdapter.Views)
                {
                    view.Rebind(() => localiser.Localise(view));
                }
            }

            foreach (var pool in ViewPoolApi.pools.Values)
            {
                foreach (var view in pool.FreeObjects)
                {
                    localiser.Localise(view);
                }
            }
        }

        private UnityViewLayer CreateUnityViewLayer(ViewLayer layer, int index)
        {
            //
            // find predefined layers
            GameObject layerObj = null;
            if (predefinedLayers != null)
            {
                foreach (var predefinedLayer in predefinedLayers)
                {
                    if (!predefinedLayer.name.Equals(layer.Id)) continue;
                    layerObj = predefinedLayer;
                    break;
                }
            }

            //
            // create if not found
            if (layerObj == null)
            {
                layerObj = new GameObject(layer.Id);
                layerObj.transform.SetParent(Root);
                var rt = layerObj.AddComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = rt.offsetMax = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
                rt.anchoredPosition3D = Vector3.zero;
                rt.localScale = Vector3.one;
            }

            var unityLayer = layerObj.AddComponent<UnityViewLayer>();
            unityLayer.Manager = this;
            unityLayer.Bind(layer);
            return unityLayer;
        }
    }
}
