using Common.Lang;
using Common.Lang.Observable;
using Common.Unity.Bind;
using Common.Unity.Util;
using Common.View;
using UnityEngine;

namespace Common.Unity.View
{
    public class UnityViewLayer : BindableMono<ViewLayer>
    {
        public UnityViewManagerAdapter Manager;
        
        public Transform Transform => gameObject.transform;
        
        public ObsListAdapter<ViewInstance, BindableMonoRaw> viewAdapter;

        public override void OnBind()
        {
            if (viewAdapter == null)
            {
                viewAdapter = new ObsListAdapter<ViewInstance, BindableMonoRaw>()
                {
                    CreateView = CreateView,
                    DisposeView = DisposeView
                };
            }
            BindBindable(Model.Views, viewAdapter);
        }

        private BindableMonoRaw CreateView(ViewInstance viewInstance, int index)
        {
            var view = viewInstance.View == null 
                ? ViewPoolApi.AddView(viewInstance.ViewId, Transform, viewInstance.Model) 
                : viewInstance.View as BindableMonoRaw;
            view.SetActive(true);
            var transform = view.transform;
            var rt = transform as RectTransform; 
            if (rt != null)
            {
                rt.localScale = Vector3.one;
                rt.offsetMin = rt.offsetMax = Vector2.zero;
            }
            else
            {
                transform.position = Vector3.zero;
            }

            viewInstance.ViewAttached(view);
            return view;
        }

        private void DisposeView(BindableMonoRaw view)
        {
            var viewInstance = view.ViewInstance;
            if (viewInstance.KeepAlive)
            {
                view.SetActive(false);
                return;
            }
            viewInstance.ViewDetached();
            ViewPoolApi.ReleaseView(view);
        }
    }
}