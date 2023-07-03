using System;

namespace Common.Lang
{
    /// <summary>
    /// Map extension that queries factory to create values for missing keys on Get() method
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class MapCache<TKey, TValue> : Map<TKey, TValue>
    {
        private readonly Func<TKey, TValue> factory;

        public MapCache(Func<TKey, TValue> factory)
        {
            this.factory = factory;
        }

        public override TValue Get(TKey key)
        {
            var val = Find(key);
            if (val == null)
            {
                this[key] = val = factory(key);
            }
            return val;
        }
        
    }
}