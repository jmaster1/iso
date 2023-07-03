using System;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.Util;

namespace Common.TimeNS
{
    /// <summary>
    /// scheduled time task
    /// </summary>
    public class TimeTask : ManagedEntity<TaskManager>, IComparable<TimeTask>
    {
        /// <summary>
        /// external manager reference, if not set, canceled/finished task will be disposed
        /// </summary>
        public object ExternalManager { get; internal set; }

        /// <summary>
        /// check if task is externally managed
        /// </summary>
        public bool External => ExternalManager != null;

        /// <summary>
        /// current state of task 
        /// </summary>
        public readonly Holder<TaskState> State = new Holder<TaskState>();
        
        /// <summary>
        /// action to execute
        /// </summary>
        public Action Action { get; internal set; }
        
        /// <summary>
        /// time when execute
        /// </summary>
        public DateTime RunTime { get; internal set; }
        
        /// <summary>
        /// task total duration
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// time left when task become paused
        /// </summary>
        internal TimeSpan PausedTimeLeft;
        
        public Time Time => Manager.Time;
        
        public bool Scheduled => State.Get() == TaskState.Scheduled;
        
        public bool Paused => State.Get() == TaskState.Paused;
        
        public bool Running => State.Get() == TaskState.Running;

        public TimeSpan TimeLeft
        {
            get
            {
                if (Scheduled) return RunTime - Time.Value;
                if (Paused) return PausedTimeLeft;
                return default;        
            }
        }

        public float TimeLeftSec => (float) TimeLeft.TotalSeconds;
        
        public float DurationSec => (float) Duration.TotalSeconds;

        public float Progress => TimeLeftSec / DurationSec;
        
        public float ProgressInverted => 1 - Progress;

        public int CompareTo(TimeTask other)
        {
            return RunTime.CompareTo(other.RunTime);
        }

        /// <summary>
        /// [re]schedule this task at specified time
        /// </summary>
        public void Schedule(DateTime when, TimeSpan duration = default)
        {
            LangHelper.Validate(Manager != null);
            Manager.Schedule(this, when, duration);
        }
        
        public void ScheduleAfterSec(float timeout)
        {
            LangHelper.Validate(Manager != null);
            Manager.ScheduleAfterSec(this, timeout);
        }
        
        public void ScheduleAfter(TimeSpan timeout, TimeSpan duration = default)
        {
            LangHelper.Validate(Manager != null);
            Manager.ScheduleAfter(this, timeout, duration);
        }

        /// <summary>
        /// safely cancel task
        /// </summary>
        /// <returns>null, so could be used in clear code: task = task?.cancel()</returns>
        public TimeTask Cancel()
        {
            LangHelper.Validate(Manager != null);
            Manager.Cancel(this);
            return null;
        }

        public void Pause()
        {
            LangHelper.Validate(Manager != null);
            Manager.Pause(this);
        }

        public void Resume()
        {
            LangHelper.Validate(Manager != null);
            Manager.Resume(this);
        }

        public override void Clear()
        {
            Manager = default;
            ExternalManager = default;
            Action = default;
            RunTime = default;
            Duration = default;
            PausedTimeLeft = default;
            State.Clear();
            base.Clear();
        }

        public override string ToString()
        {
            return $"id={Id}, state={State}, runTime={RunTime}, timeLeft={TimeLeft}";
        }
    }
}
