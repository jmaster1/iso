using System;
using System.Collections.Generic;
using Common.Lang.Collections;

namespace Common.Lang.Observable
{
    /// <summary>
    /// observable list-map
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    public class ObsListMap<TKey, TValue> : ObsList<TValue> where TValue: IIdAware<TKey>
    {
        private IdAwareMap<TKey, TValue> map = new IdAwareMap<TKey, TValue>();
        
        public IReadOnlyDictionary<TKey, TValue> Map => map;

        /// <summary>
        /// find element by key
        /// </summary>
        public TValue FindByKey(TKey key)
        {
            return map.Find(key);
        }
        
        public bool ContainsKey(TKey key)
        {
            return FindByKey(key) != null;
        }
        
        /// <summary>
        /// get element by key, throw exception if not found
        /// </summary>
        public TValue GetByKey(TKey key)
        {
            return map.Get(key);
        }
        
        public bool RemoveByKey(TKey key, out TValue value)
        {
            if (!ContainsKey(key))
            {
                value = default;
                return false;
            }
            value = GetByKey(key);
            Remove(value);
            return true;
        }
        
        protected override void AddInternal(TValue item, int index)
        {
            map.Add(item);
            base.AddInternal(item, index);
        }
        
        protected override void RemoveInternal(TValue item, int index)
        {
            map.RemoveVal(item);
            base.RemoveInternal(item, index);
        }
        
        protected override void ClearInternal()
        {
            map.Clear();
            base.ClearInternal();
        }

        /// <summary>
        /// populate target map using each element key and value provided by function
        /// </summary>
        public void WriteMap<MV>(Map<TKey, MV> target, Func<TValue, MV> valueFunc)
        {
            foreach (var e in this)
            {
                MV val = valueFunc(e);
                target[e.GetId()] = val;
            }
        }
        
        /// <summary>
        /// add all the keys to target list
        /// </summary>
        public void WriteIdList(List<TKey> target)
        {
            for(var i = 0; i < Count; i++)
            {
                var v = Get(i);
                var k = v.GetId();
                target.Add(k);
            }
        }
        
        /// <summary>
        /// create/add values for each id in source list
        /// </summary>
        /// <param name="source"></param>
        /// <param name="factory">a factory to create value from id</param>
        public void ReadIdList(List<TKey> source, Func<TKey, TValue> factory)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var k = source[i];
                var v = factory(k);
                Add(v);
            }
        }
        
        /// <summary>
        /// add missing items for each element in sourceList by matching id,
        /// this method is useful to build objects from InfoSet
        /// </summary>
        /// <param name="sourceList"></param>
        /// <param name="factory"></param>
        /// <typeparam name="TSource"></typeparam>
        public void Populate<TSource>(IEnumerable<TSource> sourceList, Func<TSource, TValue> factory)
            where TSource : IIdAware<TKey>
        {
            foreach (var source in sourceList)
            {
                var id = source.GetId();
                if (ContainsKey(id))
                {
                    continue;
                }

                var e = factory(source);
                Add(e);
            }
        }
    }
}