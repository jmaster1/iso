using System;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Common.Lang.Observable
{
    /// <summary>
    /// event data container
    /// </summary>
    public class ObsListEventData<T>
    {
        public ObsListEvent Type;
        
        public ObsList<T> List;

        public T Element;

        public int Index;
    }
    
    /// <summary>
    /// observable list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObsList<T> : ObsListBase, IList<T>, IList
    {
        public readonly Events<ObsListEvent, ObsListEventData<T>> events = new Events<ObsListEvent, ObsListEventData<T>>();

        private readonly ObsListEventData<T> eventData = new ObsListEventData<T>();
        
        private readonly List<T> list = new List<T>();
        
        public IList<T> List => list;

        /// <summary>
        /// check if list is being modified at this time
        /// </summary>
        public bool IsMutating => events.IsFiring;

        public bool IsEmpty => Count == 0;

        public void Sort(IComparer<T> comparer)
        {
            list.Sort(comparer);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) list).GetEnumerator();
        }

        public void Add(T item)
        {
            var index = Count;
            Insert(index, item);
        }

        public override void Clear()
        {
            Fire(ObsListEvent.ClearBefore);
            ClearInternal();
            Fire(ObsListEvent.ClearAfter);
        }

        protected virtual void ClearInternal()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var index = list.IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }
        
        /// <summary>
        /// remove all elements matching criteria
        /// </summary>
        /// <returns>count of removed elements</returns>
        public int Remove(Predicate<T> match)
        {
            var n = 0;
            for (var i = Count - 1; i >= 0; i--)
            {
                if (!match(this[i])) continue;
                RemoveAt(i);
                n++;
            }
            return n;
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection) list).CopyTo(array, index);
        }

        public int Count => list.Count;
        
        public bool IsSynchronized => ((ICollection) list).IsSynchronized;

        public object SyncRoot => ((ICollection) list).SyncRoot;

        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Fire(ObsListEvent.AddBefore, item, index);
            AddInternal(item, index);
            Fire(ObsListEvent.AddAfter, item, index);
        }

        public T Get(int index)
        {
            return list[index];
        }
        
        public T GetSafe(int index)
        {
            return index < 0 || index >= Count ? default : list[index];
        }

        protected virtual void AddInternal(T item, int index)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            RemoveAtGet(index);
        }
        
        public T RemoveAtGet(int index)
        {
            var item = list[index];
            Fire(ObsListEvent.RemoveBefore, item, index);
            RemoveInternal(item, index);
            Fire(ObsListEvent.RemoveAfter, item, index);
            return item;
        }

        public bool IsFixedSize => ((IList) list).IsFixedSize;

        protected virtual void RemoveInternal(T item, int index)
        {
            list.RemoveAt(index);
        }
        
        /// <summary>
        /// find first element accepted by filter
        /// </summary>
        public T FindFirst(Func<T, bool> filter)
        {
            for (var i = 0; i < Count; i++)
            {
                var e = this[i];
                if (filter(e)) return e;
            }
            return default;
        }
        
        /// <summary>
        /// find last element accepted by filter
        /// </summary>
        public T FindLast(Func<T, bool> filter)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                var e = this[i];
                if (filter(e)) return e;
            }
            return default;
        }
        
        /// <summary>
        /// find element next to specified
        /// </summary>
        public T FindNext(T item)
        {
            var index = IndexOf(item);
            if(index == -1 || index == Count - 1) return default;
            return this[index + 1];
        }
        
        public T FindNextCycled(T item)
        {
            var index = IndexOf(item);
            if(index == -1) return default;
            if (++index == Count) index = 0;
            return this[index];
        }
        
        /// <summary>
        /// find element prev to specified
        /// </summary>
        public T FindPrev(T item)
        {
            var index = IndexOf(item);
            return index <= 0 ? default : this[index - 1];
        }

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public void AddListener(Action<ObsListEvent, ObsListEventData<T>> listener, bool notify = false)
        {
            events.AddListener(listener);
            if (!notify) return;
            for (var i = 0; i < list.Count; i++)
            {
                var e = list[i];
                eventData.List = this;
                eventData.Element = e;
                eventData.Index = i;
                listener(ObsListEvent.AddAfter, eventData);
                eventData.Element = default;
            }
        }

        public void AddListenerNotify(Action<ObsListEvent, ObsListEventData<T>> listener)
        {
            AddListener(listener, true);
        }

        public void RemoveListener(Action<ObsListEvent, ObsListEventData<T>> listener)
        {
            events.RemoveListener(listener);
        }

        /// <summary>
        /// fire event
        /// </summary>
        void Fire(ObsListEvent type, T item = default, int index = -1)
        {
            LangHelper.Validate(!events.IsFiring, "Mutate in notification prohibited!");
            eventData.Type = type;
            eventData.List = this;
            eventData.Element = item;
            eventData.Index = index;
            events.Fire(type, eventData);
            eventData.Element = default;
            eventData.Type = default;
            eventData.Index = -1;
        }

        /// <summary>
        /// check if item at specified index is being removed
        /// </summary>
        public bool IsRemovingIndex(int index)
        {
            return IsMutating && eventData.Type == ObsListEvent.RemoveBefore 
                              && eventData.Index == index;
        }
        
        /// <summary>
        /// check if specified item is being removed
        /// </summary>
        public bool IsRemovingElement(T e)
        {
            return IsMutating && eventData.Type == ObsListEvent.RemoveBefore 
                              && eventData.Element.Equals(e);
        }

        public override int GetCount()
        {
            return Count;
        }

        public override object GetElement(int index)
        {
            return this[index];
        }
        
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T) value;
        }
        
        int IList.Add(object value)
        {
            list.Add((T)value);
            return Count;
        }
        
        bool IList.Contains(object value)
        {
            return list.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return list.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            list.Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            list.Remove((T)value);
        }
    }
}