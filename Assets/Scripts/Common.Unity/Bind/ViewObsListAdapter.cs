using System.Linq;
using Common.Lang.Observable;
using Common.Util;
using UnityEngine;

namespace Common.Unity.Bind
{
    /// <summary>
    /// BindableMono extension that keeps list of views in sync with list of model elements
    /// </summary>
    /// <typeparam name="TM">element type of model</typeparam>
    /// <typeparam name="TV">view type for model element</typeparam>
    public class ViewObsListAdapter<TM, TV> : BindableMono<ObsList<TM>> where TV : BindableMono<TM>
    {
        /// <summary>
        /// adapter responsible for synchronizing model/view list
        /// </summary>
        public ObsListAdapter<TM, TV> Adapter = new();
        
        [Tooltip("Container for views, will use self if not set")]
        public Transform container;
        
        public ObsList<TV> Views => Adapter.Views;

        private Transform Content => container == null ? transform : container;
        
        public override void OnBind()
        {
            Adapter.CreateView ??= CreateView;
            Adapter.DisposeView ??= DisposeView;
            //
            // cleanup child views that might exist as placeholders
            var children = Content.GetComponentsInChildren<TV>();
            if (!children.IsEmpty())
            {
                foreach (var e in children)
                {
                    ViewPoolApi.ReleaseView(e);
                }
            }
            BindBindable(Model, Adapter);
        }

        protected virtual TV CreateView(TM element, int index)
        {
            var view = ViewPoolApi.AddView<TV>(Content, element);
            view.transform.SetSiblingIndex(index);
            return view;
        }
        
        protected virtual void DisposeView(TV view)
        {
            ViewPoolApi.ReleaseView(view);
        }

        /// <summary>
        /// find view for specified model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TV FindView(TM model)
        {
            return Adapter.Views.FirstOrDefault(view => LangHelper.Equals(view.Model, model));
        }
    }
}
