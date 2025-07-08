using System;
using Common.Lang.Collections;
using Common.Lang.Entity;

namespace Common.Util
{
    /// <summary>
    /// simple cache with factory (optional, used on Get() miss)
    /// </summary>
    /// <typeparam name="TK">key type</typeparam>
    /// <typeparam name="TV">value type</typeparam>
    public class Cache<TK, TV> : GenericBean
    {
        /// <summary>
        /// factory for creating values
        /// </summary>
        private readonly Func<TK, TV> _factory;
        
        /// <summary>
        /// cached objects map
        /// </summary>
        private readonly Map<TK, TV> _map = new();

        public Cache(Func<TK, TV> factory = null)
        {
            _factory = factory;
        }

        /// <summary>
        /// get value by key, will query factory if miss
        /// </summary>
        public TV Get(TK key)
        {
            var val = _map.Find(key);
            if (val != null) return val;
            val = _factory(key);
            _map.Add(key, val);
            return val;
        }

        /// <summary>
        /// explicitly add value
        /// </summary>
        public void Put(TK key, TV val)
        {
            _map.Add(key, val);
        }
        
        /// <summary>
        /// find generic type value by key as out param without factory query on miss
        /// </summary>
        public bool Find<T>(TK key, out T val) where T: class
        {
            val = _map.Find(key) as T;
            return val != null;
        }
        
        public bool ContainsKey(TK key)
        {
            return _map.ContainsKey(key);
        }

        public override void Clear()
        {
            _map.Clear();
        }
    }
}