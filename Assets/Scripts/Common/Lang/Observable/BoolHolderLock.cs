using System;
using System.Collections.Generic;

namespace Common.Lang.Observable
{
    /// <summary>
    /// BoolHolder extension that becomes true if contains ANY object in a lock list
    /// </summary>
    public class BoolHolderLock : BoolHolder
    {
        public readonly List<object> locks = new List<object>();

        /// <summary>
        /// add lock object
        /// </summary>
        public void AddLock(object val)
        {
            locks.Add(val);
            UpdateState();
        }
        
        /// <summary>
        /// remove lock object
        /// </summary>
        public void RemoveLock(object val)
        {
            locks.Remove(val);
            UpdateState();
        }

        void UpdateState()
        {
            Set(locks.Count > 0);
        }

        public override void Clear()
        {
            locks.Clear();
            base.Clear();
        }
        
        public Action AwaitLocked(Action onComplete, bool cancelOnComplete = true)
        {
            return AwaitTrue( onComplete, cancelOnComplete);
        }
        
        public Action AwaitUnlocked(Action onComplete, bool cancelOnComplete = true)
        {
            return AwaitFalse( onComplete, cancelOnComplete);
        }
    }
}