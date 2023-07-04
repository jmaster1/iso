using System.Threading;
using Common.Bind;
using Common.ContextNS;
using Common.Lang.Observable;
using Common.Lang.Selection;
using Common.Unity.Bind;
using Common.Unity.UI.Debug;
using Common.Unity.Util;
using Common.Unity.Util.Debug;
using Common.Util.Http;
using Common.View;

namespace Common.Unity.Boot
{
    /// <summary>
    /// debug root of unicom framework.
    /// instantiated in debug mode only
    /// manages debug methods:
    /// - in-game debug button + popup containing tabbed pane of debug views
    /// - http debug 
    /// </summary>
    public class UnicomDebug : BindableBean<Unicom>
    {
        /// <summary>
        /// available after Init in debug mode
        /// </summary>
        public HttpDebug HttpDebug { get; private set; }
        
        public readonly ViewManager ViewManager = GetBean<ViewManager>();
        
        /// <summary>
        /// in-game debug button (triggers debug view popup show)
        /// </summary>
        public ViewInstance DebugButton { get; private set; }
        
        /// <summary>
        /// in-game debug popup
        /// </summary>
        public ViewInstance DebugView { get; private set; }

        /// <summary>
        /// list of tabs in DebugView
        /// </summary>
        public readonly ObsList<DebugTab> tabs = new ObsList<DebugTab>();
        
        public readonly SingleSelection<DebugTab> tabSelection = new SingleSelection<DebugTab>
        {
            Autoselect = true,
            HolderFunc = e => e.selected
        };

        protected override void OnBind()
        {
            //
            // debug button/view
            AddTab<UnicomDebug, SystemDebugView>(this, "sys");
            DebugButton = ViewManager.Create<DebugButtonView>()
                .SetModel(this)
                .SetLayerDebug();
            DebugView = ViewManager.Create<DebugView>()
                .SetKeepAlive()
                .SetModel(this)
                .SetLayerDebug();
            //
            // http debug
            HttpDebug = new HttpDebug();
            HttpDebug.Bind(Context.GetCurrent());
            BindBindable(tabs, tabSelection);
            //
            // replace handler
            HttpDebug.Server.QueryHandler = HandleQuery;
            //
            // add default http handlers
            var router = HttpDebug.Router;
            router.AddHandler<SceneHtmlAdapter>();
            router.AddHandler<LogHtmlAdapter>();
            router.AddHandler(new FileSystemHtmlAdapter(UnityHelper.PrivateFileSystem));
        }

        /// <summary>
        /// add debug tab view
        /// </summary>
        /// <param name="model">model for view</param>
        /// <param name="label">label for tab button</param>
        /// <typeparam name="TModel">type of model</typeparam>
        /// <typeparam name="TView">type of view</typeparam>
        public void AddTab<TModel, TView>(TModel model, string label) where TView : BindableMono<TModel>
        {
            var tab = new DebugTab
            {
                Manager = this,
                Model = model,
                ViewType = typeof(TView),
                Label = label
            };
            tabs.Add(tab);
        }

        /// <summary>
        /// try to handle http requests in unity thread,
        /// but handle in non-unity thread on timeout 
        /// </summary>
        private void HandleQuery(HttpQuery query)
        {
            var processed = false;

            void Run()
            {
                if (processed) return;
                processed = true;
                HttpDebug.Router.HandleQuery(query);
            }

            Unicom.RunInMain(Run);
            Thread.Sleep(1000);
            Run();
        }

        public void Select(DebugTab tab)
        {
            tabSelection.Select(tab);
        }
    }
}