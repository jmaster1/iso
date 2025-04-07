using System;
using Common.Api.Input;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.TimeNS;
using Common.Util;
using Common.Util.Http;
using Common.View;
using Newtonsoft.Json;

namespace Common.Player
{
    /// <summary>
    /// base class for player feature
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class AbstractFeature : GenericBean
    {
        /// <summary>
        ///  disabled flag, debug purposes, disabled adapter won't start
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public readonly BoolHolder Disabled = new();

        public bool IsDisabled => Disabled.Get();
        
        /// <summary>
        /// AbstractPlayer references, subclasses may define readonly
        /// property of concrete player by casting this property:
        /// public MyPlayer Player => (MyPlayer) AbstractPlayer;
        /// </summary>
        public AbstractPlayer AbstractPlayer { get; internal set; }
        
        public ViewManager ViewManager => AbstractPlayer.ViewManager;

        public InputApi InputApi => AbstractPlayer.InputApi;

        public Time TimeGame => AbstractPlayer.TimeGame;
        
        public Time TimeSystem => AbstractPlayer.TimeSystem;
        
        public TaskManager TaskManagerSystem => AbstractPlayer.TaskManagerSystem;

        public DateTime SystemTimeValue => TaskManagerSystem.TimeValue;
        
        public TaskManager TaskManagerGame  => AbstractPlayer.TaskManagerGame;
        
        /// <summary>
        /// shortcut to randomizer
        /// </summary>
        public Rnd Rnd => AbstractPlayer.Rnd;
        
        /// <summary>
        /// unsaved state (dirty) marker, indicates that state should be 
        /// </summary>
        public bool Dirty;

        public bool Loading => AbstractPlayer.Loading;
        
        /// <summary>
        /// shows whether feature contains persistent data 
        /// </summary>
        public virtual bool IsPersistent => true;

        public string Name => GetType().Name;

        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        /// called once at beginning of life
        /// </summary>
        public virtual void Init()
        {
        }
        
        /// <summary>
        /// clear state, invoked by player
        /// </summary>
        public override void Clear()
        {
            Dirty = false;
        }
        
        /// <summary>
        /// start called once after state loaded
        /// </summary>
        public virtual void Start()
        {
        }
        
        /// <summary>
        /// create externally managed task with specified action linked to TaskManagerSystem
        /// </summary>
        protected TimeTask CreateSystemTask(Action action, string taskId = null)
        {
            var task = TaskManagerSystem.CreateTask(action, this);
            task.Id = taskId;
            return task;
        }
        
        /// <summary>
        /// listener for generic holder that invokes Save() on holder change
        /// </summary>
        protected void SaveListener<T>(Holder<T> holder, T newVal, T oldVal)
        {
            DirtySet();
        }
        
        /// <summary>
        /// invoke action on next time tick
        /// </summary>
        internal void RunNextTime(Action action)
        {
            AbstractPlayer.TaskManagerSystem.ScheduleAfterSec(action, 0);
        }
        
        /// <summary>
        /// this should be called by adapter whenever state should be marked as dirty
        /// </summary>
        [HttpInvoke]
        public void DirtySet()
        {
            Dirty = true;
            AbstractPlayer.OnDirtySet(this);
        }
        
        public void DirtyReset()
        {
            Dirty = false;
        }
        
        /// <summary>
        /// disable this adapter
        /// </summary>
        public void SetDisabled(bool dis)
        {
            Disabled.Set(dis);
            DirtySet();
        }

        /// <summary>
        /// register self as http query processor
        /// </summary>
        /// <param name="httpRouter"></param>
        public virtual void RegisterHttpDebug(HttpRouter httpRouter)
        {
            httpRouter.AddHandler(this);
        }

        protected void InputLockAdd(object val)
        {
            InputApi.InputLockAdd(val);
        }
        
        protected void InputLockRemove(object val)
        {
            InputApi.InputLockRemove(val);
        }

        protected T GetView<T, TM>(TM model = null) where TM : class where T : AbstractViewController<TM>
        {
            model ??= this as TM;
            return AbstractPlayer.GetView<T, TM>(model);
        }
    }
}
