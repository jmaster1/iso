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
        private readonly List<T> _list = new(4);

        /// <summary>
        /// listeners being added/removed between Begin/End calls
        /// </summary>
        private List<T>? _added, _removed;
        
        public int Size => _list.Count;

        private int _started;

        /// <summary>
        /// shows whether notification is in progress
        /// </summary>
        public bool Started => _started > 0;

        /// <summary>
        /// check if given listener is added
        /// </summary>
        public bool Contains(T listener)
        {
            if (_removed != null && _removed.Contains(listener)) return false;
            if (_added != null && _added.Contains(listener)) return true;
            return _list.Contains(listener);
        }

        /// <summary>
        /// add new listener (must not be added)
        /// </summary>
        public void Add(T e)
        {
            LangHelper.Validate(!Contains(e));
            if (Started)
            {
                _added ??= new(2);
                _added.Add(e);
            }
            else
            {
                _list.Add(e);
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
                _removed ??= new List<T>(2);
                _removed.Add(e);
            }
            else
            {
                _list.Remove(e);
            }
        }

        /// <summary>
        /// must be called before notifying listeners
        /// </summary>
        /// <returns>number of listeners</returns>
        public int Begin()
        {
            _started++;
            return _list.Count;
        }
        
        public void End()
        {
            _started--;
            //
            // apply updates
            if (Started) return;
            if (_removed != null && _removed.Count > 0)
            {
                foreach (var e in _removed)
                {
                    _list.Remove(e);
                }

                _removed.Clear();
            }

            if (_added != null && _added.Count > 0)
            {
                foreach (var e in _added)
                {
                    _list.Add(e);
                }

                _added.Clear();
            }
        }

        public T Get(int index)
        {
            return _list[index];
        }

        public void Clear()
        {
            _added?.Clear();
            _removed?.Clear();
            _list.Clear();
        }
    }
}
