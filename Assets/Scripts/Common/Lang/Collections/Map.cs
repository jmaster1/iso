using System;
using System.Collections.Generic;
using System.Linq;
using Common.Util;

namespace Common.Lang.Collections
{
    /// <summary>
    /// Dictionary extension that is more convenient to use
    /// </summary>
    public class Map<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public Map()
        {
        }

        public Map(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// retrieve value by key (return null if not found)
        /// </summary>
        public TValue Find(TKey key)
        {
            if (key == null) return default;
            TryGetValue(key, out var val);
            return val;
        }
        
        /// <summary>
        /// retrieve value from map (validate if exists)
        /// </summary>
        public virtual TValue Get(TKey key)
        {
            var val = Find(key);
            if (val == null)
            {
                LangHelper.Throw($"Key not found: {key}");
            }
            return val;
        }

        /// <summary>
        /// remove and return value mapped by key
        /// </summary>
        public TValue RemoveGet(TKey key)
        {
            if (!ContainsKey(key)) return default;
            var ret = Get(key);
            Remove(key);
            return ret;
        }

        /// <summary>
        /// remove all the entries with accepted values
        /// </summary>
        /// <param name="filter">filter for values to remove</param>
        /// <returns>number of entries removed</returns>
        public int RemoveValues(Func<TValue, bool> filter)
        {
            var items = this.Where(
                kvp => filter(kvp.Value)).ToList();
            foreach(var item in items)
            {
                Remove(item.Key);
            }

            return items.Count;
        }
        
        /// <summary>
        /// remove all the entries with specified value
        /// </summary>
        /// <param name="val"></param>
        /// <returns>number of entries removed</returns>
        public int RemoveValue(TValue val)
        {
            return RemoveValues(value => LangHelper.Equals(value, val));
        }
    }

    /// <summary>
    /// Map extension with IdAware values
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public class IdAwareMap<TK, TV> : Map<TK, TV> where TV : IIdAware<TK>
    {
        public IdAwareMap()
        {
        }

        public IdAwareMap(int capacity) : base(capacity)
        {
        }

        public void Add(TV val)
        {
            var key = val.GetId();
            LangHelper.Validate(key != null && !ContainsKey(key));
            Add(key, val);
        }
        
        public void RemoveVal(TV val)
        {
            Remove(val.GetId());
        }
    }
}