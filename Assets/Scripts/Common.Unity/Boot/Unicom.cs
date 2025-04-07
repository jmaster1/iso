using System;
using System.Collections.Generic;
using System.Threading;
using Common.Api.Info;
using Common.Api.Local;
using Common.Api.Resource;
using Common.ContextNS;
using Common.Lang.Entity;
using Common.TimeNS;
using Common.Unity.Util;
using Common.Unity.Util.Input;
using Common.Util;
using Common.Util.Http;
using Common.Util.Log;
using Common.View;
using UnityEngine;
using Time = Common.TimeNS.Time;

namespace Common.Unity.Boot
{
    /// <summary>
    /// facade/entry point for unicom framework
    /// </summary>
    public class Unicom : MonoBehaviour
    {
        private static LogWrapper log;
        
        public static LogWrapper Log => log ??= new LogWrapper(typeof(Unicom));

        public static Unicom Instance { get; private set; }
        
        public static bool IsDebug =
#if DEBUG || DEBUG_BUILD || UNITY_EDITOR
            true;
#else
            false;
#endif

        /// <summary>
        /// available in debug mode
        /// </summary>
        public static UnicomDebug Debug;
        
        public static HttpRouter HttpDebugRouter => Debug?.HttpDebug.Router;

        /// <summary>
        /// unity main thread
        /// </summary>
        public static Thread MainThread { get; private set; }

        /// <summary>
        /// check if current thread is main
        /// </summary>
        public static bool IsMainThread => Thread.CurrentThread == MainThread;
        
        /// <summary>
        /// list of actions to execute on next Update() call
        /// </summary>
        private static readonly List<Action> UpdateActions = new();

        private static readonly List<Action> UpdateActionsSnapshot = new();
        
        /// <summary>
        /// system time - updated each frame, matches system clock
        /// </summary>
        public static readonly SystemTime SystemTime = new();

        public static readonly TaskManager SystemTimeTaskManager = new();
        
        /// <summary>
        /// in-game time - updated each frame, starts with game 
        /// </summary>
        public static readonly Time GameTime = new();

        public static readonly TaskManager GameTimeTaskManager = new();
        
        public static ViewManager ViewManager { get; private set; }
        
        public static readonly UnityInputAdapter UnityInputAdapter = new();

        /// <summary>
        /// player bootstrap component reference
        /// </summary>
        public AbstractPlayerBootstrap playerBootstrap;

        [Tooltip("Application target frame rate")]
        public int targetFrameRate = 60;

        [Tooltip("Application maximum frame delta time")]
        public float maximumDeltaTime = 0.1f;
        
        [Tooltip("Show/hide transition animation duration for UI components")]
        public float uiAnimationTime = 0.4f;
        
        [Tooltip("filtered input module used to filter user input")]
        public FilteredStandaloneInputModule filteredInputModule;

        private void Awake()
        {
            if(Log.IsDebugEnabled) Log.Debug($"{GetType().Name}.{nameof(Awake)}() - begin");
            LangHelper.Validate(Instance == null);
            Instance = this;
            Application.targetFrameRate = targetFrameRate;
            UnityEngine.Time.maximumDeltaTime = maximumDeltaTime;
            //
            // setup gameObject
            gameObject.isStatic = true;
#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
#else
            DontDestroyOnLoad(gameObject);
#endif
            //
            // view
            ViewManager = Context.Get<ViewManager>();
            playerBootstrap.SetupViewManager(ViewManager);
            //
            // debug
            if (IsDebug)
            {
                Debug = new UnicomDebug();
                Debug.Bind(this);
            }
            MainThread = Thread.CurrentThread;
            GameTimeTaskManager.Bind(GameTime);
            SystemTimeTaskManager.Bind(SystemTime);
            //
            // api, the order is important
            var resourceApi = Context.Get<ResourceApi>();
            resourceApi.StreamFactory = UnityHelper.ResourceStream;
            var infoApi = Context.Get<InfoApi>();
            infoApi.loaders.Add(resourceApi.LoadObject);
            var localApi = AbstractEntity.LocalApi = Context.Get<LocalApi>();
            localApi.MessageResolutionPolicy = IsDebug 
                ? MessageResolutionPolicy.ValueOrKey 
                : MessageResolutionPolicy.ValueOrNull;
            if (filteredInputModule != null)
            {
                UnityInputAdapter.Bind(filteredInputModule);
            }

            //
            // bind player bootstrap
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Creating player");
            }
            playerBootstrap.CreatePlayer();
            if (IsDebug)
            {
                playerBootstrap.SetupDebug(HttpDebugRouter);
                Debug.DebugButton.Show();
            }
            if(Log.IsDebugEnabled) Log.Debug($"{GetType().Name}.{nameof(Awake)}() - end");
        }

        private void Update()
        {
            LangHelper.Validate(Instance == this);
            LangHelper.Validate(MainThread == Thread.CurrentThread);
            //
            // invoke updateActions
            lock (UpdateActions)
            {
                var n = UpdateActions.Count;
                if(n > 0)
                {
                    UpdateActionsSnapshot.AddRange(UpdateActions);
                    UpdateActions.Clear();
                }
            }

            var nn = UpdateActionsSnapshot.Count;
            if (nn > 0)
            {
                for (var i = 0; i < nn; i++)
                {
                    try
                    {
                        var action = UpdateActionsSnapshot[i];
                        action?.Invoke();
                    }
                    finally
                    {
                        UpdateActionsSnapshot[i] = null;
                    }
                }
                UpdateActionsSnapshot.Clear();
            }
            //
            // update timers
            SystemTime.Update();
            var dtSec = UnityEngine.Time.deltaTime;
            var dt = TimeSpan.FromSeconds(dtSec);
            GameTime.Update(dt);

            //
            // delegate back key handling
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                HandleBack();
            }
        }

        private static void HandleBack()
        {
            var layers = ViewManager.Layers;
            for (var li = layers.Count - 1; li >= 0; li--)
            {
                var layer = layers[li];
                var views = layer.Views;
                for (var vi = views.Count - 1; vi >= 0; vi--)
                {
                    var view = views[vi];
                    if (view.View == null) continue;
                    var processed = view.View.OnBack();
                    if (processed)
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.Debug($"OnBack() processed by {view}");
                        }
                        return;
                    }
                }
            }
        }

        /*
        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.propertyTable("gameTime", gameTime.Value,
                "systemTime", systemTime.Value,
                "updateCount", updateCount);
        }*/

        /// <summary>
        /// schedule action to invoke on next update, this might be called from any thread
        /// </summary>
        public static void RunNextTime(Action action)
        {
            lock (UpdateActions)
            {
                UpdateActions.Add(action);
            } 
        }

        /// <summary>
        /// execute action immediately if current thread is main,
        /// otherwise RunNextTime(), this might be called from any thread
        /// </summary>
        /// <returns>true if current thread is main and action executed</returns>
        public static bool RunInMain(Action action)
        {
            if (IsMainThread)
            {
                action();
                return true;
            }
            RunNextTime(action);
            return false;
        }

        /// <summary>
        /// reload player form persistence
        /// </summary>
        public void Reload()
        {
            ViewManager.HideAll();
            playerBootstrap.Reload();
            if (IsDebug)
            {
                Debug.DebugButton.Show();
            }
        }
    }
}
