using System;
using System.Collections.Generic;
using Common.Api;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Collections;
using Common.Unity.Bind;
using Common.Unity.Util;
using Common.Util.Http;
using Common.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.Unity.Api.ViewPool
{
    /// <summary>
    /// responsible for instantiating/pooling ui from prefabs
    /// </summary>
    public class ViewPoolApi : AbstractApi
    {
        /// <summary>
        /// mapping view identifier to the pool
        /// </summary>
        public readonly Map<string, ViewPool> pools = new Map<string, ViewPool>();

        /// <summary>
        /// will be used as parent when creating new views and storing disposed
        /// </summary>
        private Transform container;

        /// <summary>
        /// used to localise newly created views, set to null to bypass localisation
        /// </summary>
        public ViewLocaliser Localiser = new ViewLocaliser();

        /// <summary>
        /// get or create pool for specified type
        /// </summary>
        private ViewPool GetPool(string viewId)
        {
            var pool = pools.Find(viewId);
            if (pool != null) return pool;
            var prefab = Resources.Load<BindableMonoRaw>(viewId);
            Validate(prefab != null, "Unable to resolve prefab: {0}", viewId);
            ValidateContainer();
            pool = pools[viewId] = new ViewPool(viewId, prefab, () =>
            {
                var view = Object.Instantiate(prefab, container);
                return view;
            });
            return pool;
        }

        public BindableMonoRaw AddView(string viewId, Transform parent, object model = null)
        {
            var pool = GetPool(viewId);
            var view = pool.Get();
            view.name = pool.Prefab.name;
            Localiser?.Localise(view);
            var transform = view.transform;
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            view.gameObject.SetActive(true);
            if (model != null)
            {
                view.BindRaw(model);
            }
            return view;
        }

        public TV AddView<TV>(Transform parent, object model = null) where TV : BindableMonoRaw
        {
            var viewType = typeof(TV);
            return (TV) AddView(viewType, model, parent);
        }

        public BindableMonoRaw AddView(Type viewType, object model, Transform parent)
        {
            var path = ViewManager.GetViewId(viewType);
            return AddView(path, parent, model);
        }
        
        public T AddView<T>(object model, Transform parent = null) where T : BindableMonoRaw
        {
            return AddView(typeof(T), model, parent) as T;
        }

        public void ReleaseView<T>(T view) where T : BindableMonoRaw
        {
            var viewId = ViewManager.GetViewId(view.GetType());
            var pool = GetPool(viewId);
            view.SetActive(false);
            //
            // setting parent may reset scale, need to restore
            var scale = view.transform.localScale;
            view.SetParent(container);
            view.transform.localScale = scale;
            view.Unbind();
            pool.Put(view);
        }

        public void ReleaseViews(IList<BindableMonoRaw> views)
        {
            foreach (var view in views)
            {
                ReleaseView(view);
            }
            views.Clear();
        }

        private void ValidateContainer()
        {
            if (container) return;
            var go = new GameObject("ViewPool");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            container = go.transform;
            container.SetActive(false);
            container.DontDestroyOnLoad();
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.h3("Pools");
            html.tableHeader("#", "viewId", "count");
            foreach (var kvs in pools)
            {
                var viewId = kvs.Key;
                var pool = kvs.Value;
                html.tr().tdRowNum().td(viewId).td(pool.Size).endTr();
            }

            html.endTable();
        }
    }
}