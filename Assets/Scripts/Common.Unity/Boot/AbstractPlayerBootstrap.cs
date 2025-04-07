using Common.IO.FileSystem;
using Common.Player;
using Common.Unity.View;
using Common.Util.Http;
using Common.View;
using UnityEngine;

namespace Common.Unity.Boot
{
    /// <summary>
    /// base class for player bootstrap,
    /// subclasses should load player on bind
    /// </summary>
    public abstract class AbstractPlayerBootstrap : MonoBehaviour
    {
        /// <summary>
        /// viewManagerAdapter is a view for ViewManager,
        /// this might be added as scene game objet component
        /// or will be created if reference not set
        /// </summary>
        public UnityViewManagerAdapter viewManagerAdapter;
        
        public static UnicomDebug UnicomDebug => Unicom.Debug;
        
        /// <summary>
        /// player reference retrieval, this should be called and return
        /// non-null value after CreatePlayer() call
        /// </summary>
        public abstract AbstractPlayer Player { get; }
        
        /// <summary>
        /// default filesystem for player persistence
        /// </summary>
        protected FileSystemTransaction Filesystem;

        /// <summary>
        /// request for reload player from persistence
        /// </summary>
        public abstract void Reload();
        
        /// <summary>
        /// setup ViewManager layers and bind UnityViewManagerAdapter to it
        /// </summary>
        /// <param name="viewManager"></param>
        public void SetupViewManager(ViewManager viewManager)
        {
            SetupViewLayers(viewManager);
            if (viewManagerAdapter == null)
            {
                viewManagerAdapter = gameObject.AddComponent<UnityViewManagerAdapter>();
            }
            viewManagerAdapter.Bind(viewManager);
        }

        /// <summary>
        /// subclasses should register all the ui layers for ViewManager
        /// </summary>
        /// <param name="viewManager"></param>
        protected abstract void SetupViewLayers(ViewManager viewManager);

        /// <summary>
        /// create/load/start player
        /// </summary>
        public abstract void CreatePlayer();

        /// <summary>
        /// register http debug adapters
        /// </summary>
        /// <param name="httpRouter"></param>
        public virtual void SetupDebug(HttpRouter httpRouter)
        {
            Player.RegisterHttpDebug(httpRouter);
            httpRouter.AddHandler(viewManagerAdapter);
        }
    }
}
