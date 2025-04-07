using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Util
{
    /// <summary>
    /// Random extension
    /// </summary>
    public class Rnd : Random
    {
        public Rnd()
        {
        }

        public Rnd(int seed) : base(seed)
        {
        }

        public static Rnd Instance = new();

        /// <summary>
        /// random list element retrieval
        /// </summary>
        public T RandomElement<T>(IList<T> list, bool remove = false)
        {
            if (list == null || list.Count == 0) return default;
            int index = Next(list.Count);
            T val = list[index];
            if (remove)
            {
                list.RemoveAt(index);
            }
            return val;
        }

        public T RandomElement<T>(IList<T> list, Func<T, bool> filter, bool remove = false)
        {
            if (list == null || list.Count == 0) return default;
            int n = filter == null ? list.Count : list.Count(filter);
            if (n == 0) return default;
            int index = Next(n);
            for (int i = 0; i < list.Count; i++)
            {
                var e = list[i];
                if(filter != null && !filter(e)) continue;
                if (index-- == 0)
                {
                    if (remove) list.RemoveAt(i);
                    return e;
                }
            }
            return default;
        }
        
        /// <summary>
        /// random array element retrieval
        /// </summary>
        public T RandomElement<T>(T[] array)
        {
            if (array == null || array.Length == 0) return default;
            var index = Next(array.Length);
            return array[index];
        }

        public T RandomElementRemove<T>(List<T> list)
        {
            return RandomElement(list, true);
        }
        

        /// <summary>
        /// retrieve random int using interval (including max)
        /// if max is less that min, then min returned
        /// </summary>
        public int RandomIntIncl(int min, int max)
        {
            return max <= min ? min : Next(min, max + 1);
        }
        
        /// <summary>
        /// retrieve random int using interval (excluding max)
        /// if max is less that min, then min returned
        /// </summary>
        public int RandomInt(int min, int max)
        {
            return max <= min ? min : Next(min, max);
        }

        /// <summary>
        /// retrieve random bool using probability [0..1]
        /// </summary>
        public bool RandomBool(float chance)
        {
            var val = NextDouble();
            return val <= chance;
        }

        public float RandomFloat(float max)
        {
            return (float)(NextDouble() * max);
        }
    }
}