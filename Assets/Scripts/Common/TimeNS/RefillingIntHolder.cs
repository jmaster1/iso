using System;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.Util.Http;
using Newtonsoft.Json;

namespace Common.TimeNS
{
    /// <summary>
    /// int holder with max value that is refilling with time.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RefillingIntHolder : AbstractEntityIdString
    {
        /// <summary>
        /// time (sec) of single unit recovery
        /// </summary>
        public float RecoveryTime = 1;
        
        /// <summary>
        /// this will be triggered when amount changes,
        /// arg0 - added amount (negative on subtract)
        /// arg1 - true if added by timer
        /// </summary>
        public event Action<int, bool> OnAmountChange;

        /// <summary>
        /// this will be triggered when requested to subtract too big value resulting in negative amount,
        /// arg0 - requested amount (positive), arg1 - missing amount.
        /// for example, if current amount is 2 and requested to subtract 5 units,
        /// this action will be invoked with args (5, 3)
        /// </summary>
        public event Action<int, int> OnExcessiveRequest;
        
        /// <summary>
        /// this will be triggered when need to perist state changes
        /// </summary>
        public event Action OnStateChange;
        
        /// <summary>
        /// max value
        /// </summary>
        [JsonProperty]
        public readonly IntHolder Max = new IntHolder();
        
        /// <summary>
        /// current amount holder
        /// </summary>
        [JsonProperty]
        public readonly IntHolder Current = new IntHolder();
        
        /// <summary>
        /// single unit recovery task
        /// </summary>
        [JsonProperty]
        public TimeTask RecoveryTask;
        
        /// <summary>
        /// current amount property
        /// </summary>
        public int CurrentAmount => Current.Get();
        
        /// <summary>
        /// remaining amount to max
        /// </summary>
        public int Remaining => Max.Get() - CurrentAmount;
        
        /// <summary>
        /// full bar flag
        /// </summary>
        public bool Full => CurrentAmount >= Max.Get();
        
        /// <summary>
        /// must be called once before using this object
        /// </summary>
        /// <param name="taskManager">task manager for scheduled recovery task</param>
        public void Init(TaskManager taskManager)
        {
            Validate(RecoveryTask == null);
            RecoveryTask = taskManager.CreateTask(() => Add(1), this);
        }
        
        public override void Clear()
        {
            RecoveryTask.Cancel();
        }

        public void Start()
        {
            //
            // check if we have loaded scheduled recovery task, add energy for all the time 
            if (RecoveryTask.Scheduled)
            {
                var timeLeft = RecoveryTask.TimeLeft;
                if(timeLeft.Ticks < 0 && !Full)
                {
                    var timeLeftSec = (float) timeLeft.TotalSeconds;
                    var recovered = -timeLeftSec / RecoveryTime + 1;
                    var add = (int) System.Math.Floor(recovered);
                    add = System.Math.Min(add, Remaining);
                    Add(add, true);
                    //
                    // re-schedule task if needed
                    if (!Full)
                    {
                        var delay = (1 - recovered % 1) * RecoveryTime;
                        RecoveryTask.ScheduleAfterSec(delay);
                    }
                }
            }
            SyncRecoveryTask();
        }

        /// <summary>
        /// normalized progress value
        /// </summary>
        /// <returns>[0..1]</returns>
        public float GetProgress()
        {
            var ratio = CurrentAmount / (float)Max.Get();
            return ratio < 0f ? 0f : ratio > 1f ? 1f : ratio;
        }

        private bool SyncRecoveryTask() {
            if (GetProgress() < 1)
            {
                if (RecoveryTask.Scheduled) return false;
                RecoveryTask.ScheduleAfterSec(RecoveryTime);
                OnStateChange?.Invoke();
            }
            else
            {
                if (!RecoveryTask.Scheduled) return false;
                RecoveryTask.Cancel();
                OnStateChange?.Invoke();
            }

            return true;
        }

        /// <summary>
        /// try add (or subtract) amount
        /// </summary>
        /// <param name="amount">amount to add (negative to subtract)</param>
        /// <param name="timer">true if added by timer, false if externally</param>
        /// <returns>false if not updated due to negative result value</returns>
        private bool Add(int amount, bool timer)
        {
            var resultAmount = Current.Get() + amount;
            if (resultAmount < 0)
            {
                OnExcessiveRequest?.Invoke(-amount, -resultAmount);
                return false;
            }

            Current.Add(amount);
            if (!SyncRecoveryTask())
            {
                OnStateChange?.Invoke();    
            }
            OnAmountChange?.Invoke(amount, timer);
            return true;
        }

        public bool Add(int amount)
        {
            return Add(amount, false);
        }

        [HttpInvoke]
        public void SetMax()
        {
            Add(Remaining);
        }
        
        [HttpInvoke]
        public void SetZero()
        {
            Add(-CurrentAmount);
        }
        
        [HttpInvoke]
        public bool Subtract()
        {
            return Add(-1);
        }

        public void SetValueAndMax(int value, int max)
        {
            Current.Set(value);
            Max.Set(max);
        }
    }
}