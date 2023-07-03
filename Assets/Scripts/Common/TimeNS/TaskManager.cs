using System;
using System.Collections.Generic;
using Common.Api.Pool;
using Common.Bind;
using Common.IO.Streams;
using Common.Util;
using Common.Util.Http;

namespace Common.TimeNS
{
    /// <summary>
    /// generic task manager - capable of scheduling actions
    /// in order to operate this should be bound to Time
    /// </summary>
    public class TaskManager : BindableBean<Time>
    {
        /// <summary>
        /// task pool
        /// </summary>
        public Pool<TimeTask> pool = new Pool<TimeTask>();
        
        public Time Time => Model;

        public DateTime TimeValue => Time.Value;

        /// <summary>
        /// list of scheduled tasks
        /// </summary>
        private readonly LinkedList<TimeTask> scheduledTasks = new LinkedList<TimeTask>();

        /// <summary>
        /// list of paused tasks
        /// </summary>
        private readonly LinkedList<TimeTask> pausedTasks = new LinkedList<TimeTask>();
        
        protected override void OnBind()
        {
            Model.AddListener(OnTimeUpdate);
        }

        protected override void OnUnbind()
        {
            Model.RemoveListener(OnTimeUpdate);
        }
        
        private void OnTimeUpdate(Time e)
        {
            //
            // run all the pending tasks where time has come
            while (scheduledTasks.Count > 0)
            {
                var task = scheduledTasks.First.Value;
                LangHelper.Validate(task.Scheduled);
                if (task.RunTime > Model.Value) break;
                //
                // remove and run
                if (Log.IsDebugEnabled) Log.DebugFormat("Run task {0}", task);
                scheduledTasks.RemoveFirst();
                task.State.Set(TaskState.Running);
                task.Action();
                //
                // task state might be changed during run
                if (task.State.Is(TaskState.Running))
                {
                    task.State.Set(TaskState.Finished);
                    //
                    // release task
                    if (!task.External)
                    {
                        pool.Put(task);
                    }
                }
            }
        }

        /// <summary>
        /// create new task without scheduling it
        /// </summary>
        public TimeTask CreateTask(Action action, object externalManager = null)
        {
            var task = pool.Get();
            task.Manager = this;
            task.ExternalManager = externalManager;
            task.State.Set(TaskState.Idle);
            task.Action = action;
            return task;
        }
        
        /// <summary>
        /// [re]schedule given task to run at specified time
        /// </summary>
        public void Schedule(TimeTask task, DateTime when, TimeSpan duration = default)
        {
            Cancel(task);
            if (Log.IsDebugEnabled) Log.DebugFormat("Schedule task {0} at time {1}", task, when);
            LangHelper.Validate(!scheduledTasks.Contains(task));
            task.RunTime = when;
            if (duration == default && IsBound())
            {
                duration = when - TimeValue;
            }
            task.Duration = duration;
            //
            // insert at proper position
            scheduledTasks.AddSorted(task);
            task.State.Set(TaskState.Scheduled);
        }
        
        public void ScheduleAfterSec(TimeTask task, float timeout, TimeSpan duration = default)
        {
            var when = TimeValue + TimeSpan.FromSeconds(timeout);
            Schedule(task, when);
        }
        
        public void ScheduleAfter(TimeTask task, TimeSpan timeout, TimeSpan duration = default)
        {
            var when = TimeValue + timeout;
            Schedule(task, when, duration);
        }

        /// <summary>
        /// create and schedule new task
        /// </summary>
        public TimeTask Schedule(Action action, DateTime when)
        {
            var task = CreateTask(action);
            Schedule(task, when);
            return task;
        }

        public TimeTask ScheduleAfterSec(Action action, float timeout)
        {
            var when = TimeValue + TimeSpan.FromSeconds(timeout);
            return Schedule(action, when);
        }

        public void UnbindAndClear()
        {
            Unbind();
            Clear();
        }
        
        /// <summary>
        /// cancel all scheduled tasks
        /// </summary>
        public override void Clear()
        {
            CancelTasks(scheduledTasks);
            CancelTasks(pausedTasks);
        }

        void CancelTasks(LinkedList<TimeTask> tasks)
        {
            while (tasks.Count > 0)
            {
                var task = tasks.Last.Value;
                Cancel(task);
            }
        }

        /// <summary>
        /// safely cancel specified task
        /// </summary>
        public void Cancel(TimeTask task)
        {
            LinkedList<TimeTask> list = null;
            if (task.Scheduled) list = scheduledTasks;
            else if (task.Paused) list = pausedTasks;
            if (list == null) return;
            if (Log.IsDebugEnabled) Log.DebugFormat("Cancel task {0}", task);
            var removed = list.Remove(task);
            LangHelper.Validate(removed);
            task.State.Set(TaskState.Cancelled);
            task.RunTime = default;
            if (!task.External)
            {
                pool.Put(task);
            }
        }
        
        /// <summary>
        /// pause specified task, must be scheduled
        /// </summary>
        public void Pause(TimeTask task)
        {
            if (!task.Scheduled) return;
            if (Log.IsDebugEnabled) Log.DebugFormat("Pause task {0}", task);
            scheduledTasks.RemoveValidate(task);
            task.PausedTimeLeft = task.TimeLeft;
            task.State.Set(TaskState.Paused);
            pausedTasks.AddLast(task);
        }
        
        /// <summary>
        /// safely resume specified task
        /// </summary>
        public void Resume(TimeTask task)
        {
            if (!task.Paused) return;
            if (Log.IsDebugEnabled) Log.DebugFormat("Resume task {0}", task);
            pausedTasks.RemoveValidate(task);
            task.RunTime = TimeValue + task.PausedTimeLeft;
            task.PausedTimeLeft = default;
            task.State.Set(TaskState.Scheduled);
            scheduledTasks.AddSorted(task);
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.propertyTable("TimeValue", TimeValue);
            html.tableHeader("#", "id", "state", "runTime");
            foreach (var e in scheduledTasks)
            {
                html.tr().tdRowNum().td(e.Id).td(e.State.Get()).td(e.RunTime).endTr();
            }
            html.endTable();
        }
    }
}
