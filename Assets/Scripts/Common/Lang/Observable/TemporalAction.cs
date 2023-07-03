using System;
using System.Threading.Tasks;
using Common.Lang.Entity;

namespace Common.Lang.Observable
{
    /// <summary>
    /// manages temporal (time consuming) action execution.
    /// should be used to decouple model (request action) and view (executes action).
    /// Workflow:
    /// 1. model calls RequestAction, passing its' type, payload, and completion callback
    /// 2. request propagated to registered listeners via event of specified type
    /// 3. on event executor should call ActionStarted (if it is capable of handling this action type)
    /// 4. when execution completed, executor should call ActionCompleted
    /// 5. when all executors completed, completion callback (passed on step 1) will be invoked
    /// </summary>
    /// <typeparam name="TType">action type</typeparam>
    /// <typeparam name="TPayload">action payload</typeparam>
    public class TemporalAction<TType, TPayload> : GenericBean where TType: Enum
    {
        /// <summary>
        /// executors should listen to events
        /// </summary>
        public readonly Events<TType, TPayload> events = new Events<TType, TPayload>();

        /// <summary>
        /// type of action in progress
        /// </summary>
        public TType CurrentAction { get; private set; }
        
        public bool CurrentActionSet { get; private set; }
        
        public readonly BoolHolderLock actionLock = new BoolHolderLock();

        /// <summary>
        /// should be called by action executor upon event arrival
        /// </summary>
        public void ActionStarted(object executor)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"ActionStarted, action={CurrentAction}, executor={executor}");
            }
            Validate(CurrentActionSet);
            actionLock.AddLock(executor);
        }
        
        /// <summary>
        /// should be called by action executor upon action execution completion
        /// </summary>
        public void ActionCompleted(object executor)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"ActionCompleted, action={CurrentAction}, executor={executor}");
            }
            Validate(CurrentActionSet);
            actionLock.RemoveLock(executor);
        }
        
        /// <summary>
        /// called by model to request action execution
        /// </summary>
        /// <param name="action"></param>
        /// <param name="payload"></param>
        /// <param name="onComplete">callback to invoke upon execution completion by all executors</param>
        public Task RequestAction(TType action, TPayload payload, Action onComplete)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"RequestAction, action={action}");
            }
            var promise = new TaskCompletionSource<object>(); 
            Validate(!CurrentActionSet, CurrentAction.ToString());
            CurrentAction = action;
            CurrentActionSet = true;
            events.Fire(action, payload);
            actionLock.AwaitUnlocked(() =>
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"RequestAction:completed, action={action}");
                }
                CurrentAction = default;
                CurrentActionSet = false;
                onComplete?.Invoke();
                promise.SetResult(null);
            });
            return promise.Task;
        }
    }
}