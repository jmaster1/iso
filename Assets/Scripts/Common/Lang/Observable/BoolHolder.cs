using System;

namespace Common.Lang.Observable
{
    public class BoolHolder : Holder<bool>
    {
        public BoolHolder()
        {
        }
        
        public BoolHolder(bool val)
        {
            Set(val);
        }

        public void SetTrue()
        {
            Set(true);
        }
        
        public void SetFalse()
        {
            Set(false);
        }

        public bool Toggle()
        {
            return Set(!Get());
        }

        public Action AwaitTrue(Action onComplete, bool cancelOnComplete = true)
        {
            return AwaitValue(true, onComplete, cancelOnComplete);
        }
        
        public Action AwaitFalse(Action onComplete, bool cancelOnComplete = true)
        {
            return AwaitValue(false, onComplete, cancelOnComplete);
        }
    }
}