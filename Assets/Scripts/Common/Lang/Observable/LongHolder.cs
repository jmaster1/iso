namespace Common.Lang.Observable
{
    public class LongHolder : Holder<long>
    {
        public LongHolder()
        {
            Set(0);
        }
        
        public override long Get()
        {
            return value ^ GetHashCode();
        }

        protected override void SetInternal(long v)
        {
            value = v ^ GetHashCode();
        }
        
        public long Add(long amount)
        {
            return Set(Get() + amount);
        }
        
        public long Mul(int multiplier)
        {
            return Set(Get() * multiplier);
        }
    }
}