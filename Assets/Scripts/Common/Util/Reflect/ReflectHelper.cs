using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Common.Lang.Observable;

namespace Common.Util.Reflect
{
    /// <summary>
    /// reflection utility class
    /// </summary>
    public static class ReflectHelper
    {
        public static T NewInstance<T>()
        {
            var type = typeof(T);
            return (T) NewInstance(type);
        }
        
        public static object NewInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// field retrieval with validation
        /// </summary>
        public static FieldInfo GetField(Type type, string fieldName)
        {
            LangHelper.Validate(type != null);
            LangHelper.Validate(fieldName != null);
            var field = type.GetField(fieldName);
            LangHelper.Validate(field != null, "Field {0}.{1} not found",
                type.FullName, fieldName);
            return field;
        }

        /// <summary>
        /// copy all field values from source to target
        /// </summary>
        public static void CopyFields<T>(T from, T to) where T : class
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields();
            foreach (var f in fields)
            {
                object val = f.GetValue(from);
                f.SetValue(to, val);
            }
        }

        public static bool IsEnum(this object o)
        {
            return o is Enum;
        }

        public static bool IsNumericType(this object o)
        {
            if (o == null) return false;
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public const BindingFlags DefaultBindingFlags = BindingFlags.Instance |
                                                       BindingFlags.NonPublic |
                                                       BindingFlags.Public;

        public const char PathSeparator = '/';
        
        /// <summary>
        /// resolve text path from source object to target
        /// </summary>
        /// <param name="maxDepth">max depth to prevent infinite recursion</param>
        public static string ResolvePath(object from, object to, int maxDepth = 3)
        {
            if (from == null || to == null) return null;
            var path = new LinkedList<string>();
            ResolvePath(from, to, maxDepth, path, 0);
            return StringHelper.Join(path, PathSeparator);
        }

        static bool ResolvePath(object from, object to, int maxDepth, LinkedList<string> path, int depth)
        {
            LangHelper.Validate(from != null);
            LangHelper.Validate(to != null);
            var type = from.GetType();
            var fields = type.GetFields(DefaultBindingFlags);
            foreach (var field in fields)
            {
                var fieldName = field.Name;
                var fieldVal = field.GetValue(@from);
                if(fieldVal == null) continue;
                if (fieldVal == to) return AddToPath(path, fieldName);
            }
            if (depth >= maxDepth) return false;
            //
            // recurse
            foreach (var field in fields)
            {
                var fieldVal = field.GetValue(@from);
                if(fieldVal == null) continue;
                var fieldName = field.Name;
                if (fieldVal is IList obsList)
                {
                    for ( int j = 0, n = obsList.Count; j < n; j++)
                    {
                        var e = obsList[j];
                        if (e == to) return AddToPath(path, $"{fieldName}{PathSeparator}{j}");
                        if (e == null) continue;
                        if (ResolvePath(e, to, maxDepth, path, depth + 1))
                        {
                            return AddToPath(path, $"{fieldName}{PathSeparator}{j}");
                        }
                    }
                }
                if (ResolvePath(fieldVal, to, maxDepth, path, depth + 1))
                {
                    return AddToPath(path, fieldName);
                }
            }
            return false;
        }

        static bool AddToPath(LinkedList<string> list, string element)
        {
            list.AddFirst(element);
            return true;
        }

        /// <summary>
        /// resolve object from path
        /// </summary>
        /// <param name="from">path root object</param>
        /// <param name="path">path to target object</param>
        public static object ResolveObject(object from, string path)
        {
            if (path.IsNullOrEmpty()) return from;
            var split = path.Split(PathSeparator);
            return ResolveObject(from, split, 0);
        }

        static object ResolveObject(object from, string[] split, int index)
        {
            object ret = null;
            var n = split.Length;
            var el = split[index];
            if (el.IsDigitsOnly())
            {
                var elIndex = int.Parse(el);
                if (from is ObsListBase obsList) ret = obsList.GetElement(elIndex);
            }
            else
            {
                var type = from.GetType();
                var field = type.GetField(el);
                if (field == null) return null;
                ret = field.GetValue(from);
            }
            if(index < n - 1) ret = ResolveObject(ret, split, index + 1);
            return ret;
        }

        public static void InvokeSafe(object target, string methodName, params object[] args)
        {
            if(target == null) return;
            var type = target.GetType();
            var method = type.GetMethod(methodName, DefaultBindingFlags);
            if (method != null) method.Invoke(target, args);
        }

        public static MethodInfo FindMethod(object target, string methodName)
        {
            return target.GetType().GetMethod(methodName, DefaultBindingFlags);
        }
    }
}
