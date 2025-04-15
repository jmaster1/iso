using System;
using System.Collections.Generic;
using System.Threading;

namespace Common.Util.Threading
{
    public class ThreadedTaskQueue : IDisposable
    {
        Queue<Action> tasks = new Queue<Action>();
        
        List<Thread> workers = new List<Thread>();
        
        EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.AutoReset);

        public ThreadedTaskQueue(int workerCount, string prefix)
        {
            for (var i = 0; i < workerCount; i++)
            {
                var w = new Thread(Worker);
                w.Name = prefix + i;
                w.Start();
                workers.Add(w);
            }
        }

        void Worker()
        {
            try
            {
                while (true)
                {
                    wait.WaitOne();
                    var task = PopTask();
                    if (task == null)
                        continue;
                    try
                    {
                        task();
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        public void PushTask(Action task)
        {
            lock (tasks)
            {
                tasks.Enqueue(task);
            }

            wait.Set();
        }

        public Action PopTask()
        {
            lock (tasks)
            {
                if (tasks.Count > 0)
                {
                    var task = tasks.Dequeue();
                    wait.Set();
                    return task;
                }
            }

            return null;
        }

        public void Dispose()
        {
            foreach (var w in workers)
            {
                w.Abort();
            }

            workers.Clear();
        }
    }
}