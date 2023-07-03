using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common.Api.Pool
{
    public class DebugPool<T> : Pool<T> where T : class
    {
        public List<T> UsedObjects { get; } = new List<T>(16);

        public int AssertSizeMax { get; set; } = 0;

        public int SnapshotSize { get; private set; } = 0;

        public int UseCount { get; private set; } = 0;

        public int Delta
        {
            get => UsedObjects.Count + FreeObjects.Count - SnapshotSize;
        }

        public DebugPool(Type type, Func<T> factory) : base(type, factory)
        {
        }

        public DebugPool(Type type) : base(type)
        {
        }

        public override T Get()
        {
            T result = base.Get();
            Debug.Assert(!UsedObjects.Contains(result));
            UsedObjects.Add(result);
            Debug.Assert(!UsedObjects.Contains(result));
            Debug.Assert(AssertSize());
            return result;
        }

        private bool AssertSize()
        {
            Debug.Assert(AssertSizeMax <= Size,
                $"Pool size assertion failed, max={AssertSizeMax}, actual={Size}");
            return true;
        }

        public override void Put(T value)
        {
            Debug.Assert(value != null);
            Debug.Assert(IsUsed(value), value.GetType().Name);
            Debug.Assert(!IsFree(value));
            UsedObjects.Remove(value);
            ++UseCount;
            base.Put(value);
        }

        private bool IsUsed(T value)
        {
            return UsedObjects.Contains(value);
        }

        private bool IsFree(T value)
        {
            return FreeObjects.Contains(value);
        }

        private void UpdateSnapshotSize()
        {
            SnapshotSize = UsedObjects.Count + FreeObjects.Count;
        }
    }
}