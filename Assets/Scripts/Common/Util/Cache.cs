using System;
using Common.Lang;
using Common.Lang.Entity;

namespace Common.Util
{
    /// <summary>
    /// simple cache with factory (optional, used on Get() miss)
    /// </summary>
    /// <typeparam name="K">key type</typeparam>
    /// <typeparam name="V">value type</typeparam>
    public class Cache<K, V> : GenericBean
    {
        /// <summary>
        /// factory for creating values
        /// </summary>
        public Func<K, V> Factory;
        
        /// <summary>
        /// cached objects map
        /// </summary>
        readonly Map<K, V> map = new Map<K, V>();

        public Cache(Func<K, V> factory = null)
        {
            Factory = factory;
        }

        /// <summary>
        /// get value by key, will query factory if miss
        /// </summary>
        public V Get(K key)
        {
            var val = map.Find(key);
            if (val != null) return val;
            val = Factory(key);
            map.Add(key, val);
            return val;
        }

        /// <summary>
        /// explicitly add value
        /// </summary>
        public void Put(K key, V val)
        {
            map.Add(key, val);
        }
        
        /// <summary>
        /// find generic type value by key as out param without factory query on miss
        /// </summary>
        public bool Find<T>(K key, out T val) where T: class
        {
            val = map.Find(key) as T;
            return val != null;
        }
        
        public bool ContainsKey(K key)
        {
            return map.ContainsKey(key);
        }

        public override void Clear()
        {
            map.Clear();
        }
    }
}