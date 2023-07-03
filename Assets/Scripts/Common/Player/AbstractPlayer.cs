using System;
using System.Collections.Generic;
using System.Linq;
using Common.Api.Input;
using Common.Bind;
using Common.ContextNS;
using Common.IO.Streams;
using Common.Lang;
using Common.TimeNS;
using Common.Util;
using Common.Util.Http;
using Common.Util.Reflect;
using Common.View;

namespace Common.Player
{
    public abstract class AbstractPlayer : BindableBean<Time>
    {
        /// <summary>
        /// input api should be used to lock input during transitions 
        /// </summary>
        public readonly InputApi InputApi = GetBean<InputApi>();
        
        /// <summary>
        /// shortcut to ViewManager, should be used to manage views
        /// </summary>
        public readonly ViewManager ViewManager = Context.Get<ViewManager>();
            
        public readonly Time TimeSystem = new SystemTime();
        
        public readonly Time TimeGame = new Time();
        
        /// <summary>
        /// system time task manager
        /// </summary>
        public readonly TaskManager TaskManagerSystem = new TaskManager();
        
        /// <summary>
        /// game time task manager
        /// </summary>
        public readonly TaskManager TaskManagerGame = new TaskManager();
        
        /// <summary>
        /// randomizer to be used by features
        /// </summary>
        public readonly Rnd Rnd = new Rnd();
        
        /// <summary>
        /// all the player features
        /// </summary>
        public readonly List<AbstractFeature> features = new List<AbstractFeature>();

        public bool Loading;

        /// <summary>
        /// feature dirty state observers should register here 
        /// </summary>
        public event Action<AbstractFeature> OnDirty;
        
        /// <summary>
        /// cached view controllers
        /// </summary>
        protected readonly Map<Type, IViewController> ViewCache = new Map<Type, IViewController>();

        /// <summary>
        /// subclasses should return array of its' features, order is important
        /// </summary>
        protected abstract IEnumerable<AbstractFeature> GetFeatures();

        protected AbstractPlayer()
        {
            var feats = GetFeatures();
            foreach (var e in feats)
            {
                e.AbstractPlayer = this;
                features.Add(e);
            }
            foreach (var e in features)
            {
                e.Init();
            }
        }

        public override void Clear()
        {
            ViewManager.HideAll();
            foreach (var feature in features)
            {
                feature.Clear();
            }
            TaskManagerGame.Clear();
            TaskManagerSystem.Clear();
            base.Clear();
        }

        protected override void OnBind()
        {
            base.OnBind();
            BindBindable(Model, TimeGame);
            BindBindable(TimeGame, TaskManagerGame);
            BindBindable(Model, TimeSystem);
            BindBindable(TimeSystem, TaskManagerSystem);
        }

        /// <summary>
        /// clear and load all the features
        /// </summary>
        /// <param name="featureLoader">will be invoked for each feature to load</param>
        public void Load(Action<AbstractFeature> featureLoader)
        {
            Validate(!Loading);
            Clear();
            Loading = true;
            try
            {
                foreach (var e in features.Where(
                    e => e.IsPersistent && !e.IsDisabled))
                {
                    featureLoader(e);
                }
            }
            finally
            {
                Loading = false;
            }
        }

        /// <summary>
        /// save features
        /// </summary>
        /// <param name="dirtyOnly">shows whether dirty only should be saved</param>
        /// <param name="featureSaver">feature to save consumer (does save work)</param>
        /// <returns>number of features saved</returns>
        public int Save(bool dirtyOnly, Action<AbstractFeature> featureSaver)
        {
            var savedCount = 0;
            foreach (var e in features.Where(
                e => e.IsPersistent 
                     && !e.IsDisabled 
                     && (!dirtyOnly || e.Dirty)))
            {
                featureSaver(e);
                savedCount++;
                if (dirtyOnly) e.DirtyReset();
            }

            return savedCount;
        }
        
        public void Start()
        {
            Validate(IsBound(), "Must be bound before Start");
            foreach (var e in features.Where(e => !e.IsDisabled))
            {
                e.Start();
            }
        }
        
        /// <summary>
        /// register player and features as http router handlers 
        /// </summary>
        public void RegisterHttpDebug(HttpRouter httpRouter)
        {
            httpRouter.AddHandler(this);
            foreach (var e in features)
            {
                e.RegisterHttpDebug(httpRouter);
            }
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.h3("Features");
            html.tableHeader("#", "name", "persistent", "disabled", "dirty");
            foreach (var e in features)
            {
                html.tr()
                    .tdRowNum()
                    .td(e.Name)
                    .td(e.IsPersistent)
                    .td(e.IsDisabled)
                    .td(e.Dirty)
                    .endTr();
            }

            html.endTable();
        }

        /// <summary>
        /// called by feature when its' dirty flag is set
        /// </summary>
        internal void OnDirtySet(AbstractFeature feature)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"OnDirtySet({feature})");
            }
            OnDirty?.Invoke(feature);
        }
        
        internal T GetView<T, TM>(TM model = null) where TM : class where T : AbstractViewController<TM>
        {
            var type = typeof(T);
            var result = ViewCache.Find(type);
            if (result != null) return result as T;
            var controller = ReflectHelper.NewInstance<T>();
            controller.ViewManager = ViewManager;
            model ??= this as TM;
            controller.Bind(model);
            ViewCache[type] = controller;
            return controller;
        }
    }
}