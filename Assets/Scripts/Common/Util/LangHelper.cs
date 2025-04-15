using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Lang;
using Common.Lang.Entity;

namespace Common.Util
{
    /// <summary>
    /// language utility class
    /// </summary>
    public static class LangHelper
    {
        /// <summary>
        /// throw exception if condition is false
        /// </summary>
        public static void Validate(bool condition, string message = null)
        {
            if (!condition)
            {
                Throw(message);
            }
        }
        
        public static void Validate(bool condition, Func<string> messageFunc)
        {
            if (condition) return;
            var message = messageFunc();
            Throw(message);
        }

        public static void Validate(this AbstractEntity obj, bool condition, string message = null)
        {
            Validate(condition, message);
        }

        public static void Validate(bool condition, string format, object arg0,
            object arg1 = null, object arg2 = null, object arg3 = null, object arg4 = null)
        {
            if (condition) return;
            var message = StringHelper.Format(format, arg0, arg1, arg2, arg3, arg4);
            Validate(false, message);
        }

        /// <summary>
        /// throw exception with message (if any)
        /// </summary>
        public static void Throw(string message = null)
        {
            throw message == null ? new Exception() : new Exception(message);
        }
        
        public static void Throw(string message, Exception reason)
        {
            throw new Exception(message, reason);
        }
        
        /// <summary>
        /// retrieve enum ordinal 0-based index
        /// </summary>
        public static int Ordinal<T>(this T val) where T : Enum
        {
            return Convert.ToInt32(val);
        }

        /// <summary>
        /// retrieve enum values
        /// </summary>
        public static T[] EnumValues<T>() where T : Enum
        {
            return (T[]) Enum.GetValues(typeof(T));
        }
        
        /// <summary>
        /// retrieve enum values length
        /// </summary>
        public static int EnumLength<T>() where T : Enum
        {
            return EnumValues<T>().Length;
        }
        
        /// <summary>
        /// parse enum value from name
        /// </summary>
        public static T EnumParse<T>(string name) where T : Enum
        {
            return (T) Enum.Parse(typeof(T), name);
        }

        /// <summary>
        /// remove last element and return it (if any)
        /// </summary>
        /// <returns>null if list is epmty</returns>
        public static T Pop<T>(this IList<T> list) where T : class
        {
            if (list.Count == 0) return null;
            
            var lastElementIndex = list.Count - 1;
            var lastElement = list[lastElementIndex];
            list.RemoveAt(lastElementIndex);
            return lastElement;
        }
        
        /// <summary>
        /// add element to linked list at proper position
        /// </summary>
        public static void AddSorted<T>(this LinkedList<T> list, T e) where T: IComparable<T>
        {
            if (list.Count == 0)
            {
                list.AddFirst(e);
            }
            else
            {
                var added = false;
                for (var node = list.First; node != null; node = node.Next)
                {
                    var comp = e.CompareTo(node.Value);
                    if (comp >= 0) continue;
                    list.AddBefore(node, e);
                    added = true;
                    break;
                }
                if (!added)
                {
                    list.AddLast(e);
                }
            }
        }
        
        /// <summary>
        /// add element to linked list at proper position with comparator
        /// </summary>
        public static void AddSorted<T>(this LinkedList<T> list, T e, Func<T, int> comparator)
        {
            if (list.Count == 0)
            {
                list.AddFirst(e);
            }
            else
            {
                var added = false;
                for (var node = list.First; node != null; node = node.Next)
                {
                    var comp = comparator.Invoke(e);
                    if (comp >= 0) continue;
                    list.AddBefore(node, e);
                    added = true;
                    break;
                }
                if (!added)
                {
                    list.AddLast(e);
                }
            }
        }
        
        /// <summary>
        /// transform source list elements into target list elements
        /// </summary>
        public static void BuildList<T, S>(List<T> target, List<S> source, Func<S, T> transformer)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var s = source[i];
                var t = transformer(s);
                target.Add(t);
            }
        }

        /// <summary>
        /// add element to list if not already there
        /// </summary>
        /// <returns>true if added</returns>
        public static bool AddIfMissing<T>(this List<T> list, T element)
        {
            if (list.Contains(element)) return false;
            list.Add(element);
            return true;
        }
        
        /// <summary>
        /// remove and return element at index
        /// </summary>
        public static T RemoveAtGet<T>(this List<T> list, int index)
        {
            T ret = list[index];
            list.RemoveAt(index);
            return ret;
        }
        
        /// <summary>
        /// remove element with validation
        /// </summary>
        public static void RemoveValidate<T>(this LinkedList<T> list, T element)
        {
            var removed = list.Remove(element);
            Validate(removed);
        }
        
        /// <summary>
        /// remove duplicate elements from list
        /// </summary>
        /// <returns>number of removed items</returns>
        public static int RemoveDuplicates<T>(this List<T> list)
        {
            if (list == null) return 0;
            var n = list.Count;
            var removed = 0;
            for (var i = 0; i < n; i++)
            {
                var ei = list[i];
                for (var j = i + 1; j < n; j++)
                {
                    var ej = list[j];
                    if (!Equals(ei, ej)) continue;
                    list.RemoveAt(j);
                    n--;
                    removed++;
                }
            }
            return removed;
        }
        
        /// <summary>
        /// check if list is null or empty
        /// </summary>
        public static bool IsEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }
        
        public static int ToInt(this bool val)
        {
            return val ? 1 : 0;
        }
        
        /// <summary>
        /// stub to return passed value
        /// </summary>
        public static T NullTransform<T>(T val)
        {
            return val;
        }

        /// <summary>
        /// retrieve id of IdAware
        /// </summary>
        public static I GetId<I, T>(T val) where T: IIdAware<I>
        {
            return val.GetId();
        }
        
        /// <summary>
        /// retrieve id of IdAware
        /// </summary>
        public static string GetId<T>(T val) where T: IIdAware<string>
        {
            return val.GetId();
        }
        
        public static void ForEach<T>(this IReadOnlyList<T> src, Action<T> action)
        {
            foreach (var element in src)
            {
                action.Invoke(element);
            }
        }

        /// <summary>
        /// wrap source/throw new exception with formatted message 
        /// </summary>
        public static void Handle(Exception source, string format, params object[] args)
        {
            var message = string.Format(format, args);
            throw new Exception(message, source);
        }
        
        /// <summary>
        /// remove all elements in this list containing in an items
        /// </summary>
        /// <returns>number of removed items</returns>
        public static int RemoveAll<T>(this List<T> list, IEnumerable<T> items)
        {
            return items.Count(list.Remove);
        }

        public static bool IsEmpty<T>(T[] objects)
        {
            return objects == null || objects.Length == 0;
        }

        /// <summary>
        /// Check passed IEnumerable<T> for specific item specified into selector 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="selector"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> enumerable, Func<T, bool> selector)
        {
            return enumerable.Any(selector.Invoke);
        }
        
        public static bool Contains<T>(this T[] array, T element)
        {
            if (array == null) return false;
            var comparer = EqualityComparer<T>.Default;
            return array.Any(t => comparer.Equals(t, element));
        }

        public static bool Equals<T>(T v0, T v1)
        {
            return EqualityComparer<T>.Default.Equals(v0, v1);
        }
        
        public static bool IsNotEmpty(this Array objects)
        {
            return objects != null && objects.Length > 0;
        }
        
        public static bool IsEmpty(this Array objects)
        {
            return !IsNotEmpty(objects);
        }

        public static string ToDebugString<TK, TV>(this IDictionary<TK, TV> src)
        {
            if (src == null) return "null";
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            foreach (var entry in src)
            {
                if (sb.Length > 1) sb.Append(", ");
                sb.Append(entry.Key).Append("=").Append(entry.Value);
            }
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// action that does nothing
        /// </summary>
        public static void VoidAction()
        {
        }
        
        /// <summary>
        /// safely retrieve list element at specified index
        /// </summary>
        public static T GetSafe<T>(this IList<T> list, int index)
        {
            if(index < 0 || list == null || list.Count <= index) return default;
            return list[index];
        }
        
        public static long DoubleIntToLong(int i1, int i2)
        {
            long b = i2;
            b <<= 32;
            b |= (uint)i1;
            return b;
        }
        
        public static void LongToDoubleInt(long a, out int i1, out int i2) {
            i1 = (int)(a & uint.MaxValue);
            i2 = (int)(a >> 32);
        }

        public static bool IsCompletedOk(this Task task)
        {
            return !task.IsCanceled && !task.IsFaulted && task.IsCompleted;
        }

        /// <summary>
        /// return 1st non-null argument
        /// </summary>
        public static T Nvl<T>(T v0, T v1, T v2 = null, T v3 = null, T v4 = null) where T : class
        {
            if (v0 != null)
            {
                return v0;
            }
            
            if (v1 != null)
            {
                return v1;
            }
            
            if (v2 != null)
            {
                return v2;
            }
            
            if (v3 != null)
            {
                return v3;
            }
            
            if (v4 != null)
            {
                return v4;
            }

            return null;
        }

        public static IDisposable CreateDisposable(Action action)
        {
            return new DisposableAction(action);
        }
    }
}
