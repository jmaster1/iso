using System.Collections.Generic;
using Common.Util;

namespace Common.Lang.Observable
{
    /// <summary>
    /// generic collection of listeners, elements might be added/removed inbetween Begin/End calls,
    /// but current queue won't be affected until End() call. Sample usage for event propagation:
    /// for(int i = 0, n = listeners.Begin(); i < n; i++)
    /// {
    ///    T listener = listeners.Get(i);
    ///    listener();
    /// }
    /// listeners.End();
    /// </summary>
    /// <typeparam name="T">listener type</typeparam>
    public class Listeners<T> : IClearable
    {
        /// <summary>
        /// list of active listeners
        /// </summary>
        private readonly List<T> list = new List<T>(4);

        /// <summary>
        /// listeners being added/removed between Begin/End calls
        /// </summary>
        private List<T> added, removed;
        
        public int Size => list.Count;

        private int started;

        /// <summary>
        /// shows whether notification is in progress
        /// </summary>
        public bool Started => started > 0;

        /// <summary>
        /// check if given listener is added
        /// </summary>
        public bool Contains(T listener)
        {
            if (removed != null && removed.Contains(listener)) return false;
            if (added != null && added.Contains(listener)) return true;
            return list.Contains(listener);
        }

        /// <summary>
        /// add new listener (must not be added)
        /// </summary>
        public void Add(T e)
        {
            LangHelper.Validate(!Contains(e));
            if (Started)
            {
                added ??= new List<T>(2);
                added.Add(e);
            }
            else
            {
                list.Add(e);
            }
        }
        
        public void AddSafe(T e)
        {
            if(!Contains(e)) Add(e);
        }

        /// <summary>
        /// safely remove listener (might not be added)
        /// </summary>
        public void Remove(T e)
        {
            if (Started)
            {
                removed ??= new List<T>(2);
                removed.Add(e);
            }
            else
            {
                list.Remove(e);
            }
        }

        /// <summary>
        /// must be called before notifying listeners
        /// </summary>
        /// <returns>number of listeners</returns>
        public int Begin()
        {
            started++;
            return list.Count;
        }
        
        public void End()
        {
            started--;
            //
            // apply updates
            if (Started) return;
            if (removed != null && removed.Count > 0)
            {
                foreach (var e in removed)
                {
                    list.Remove(e);
                }

                removed.Clear();
            }

            if (added != null && added.Count > 0)
            {
                foreach (var e in added)
                {
                    list.Add(e);
                }

                added.Clear();
            }
        }

        public T Get(int index)
        {
            return list[index];
        }

        public void Clear()
        {
            added.Clear();
            removed.Clear();
            list.Clear();
        }
    }
}
