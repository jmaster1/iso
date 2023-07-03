namespace Common.Lang.Observable
{
    public class IntHolder : Holder<int>
    {
        public IntHolder()
        {
            Set(0);
        }
        
        public IntHolder(int val)
        {
            Set(val);
        }

        public override int Get()
        {
            return value ^ GetHashCode();
        }

        protected override void SetInternal(int v)
        {
            value = v ^ GetHashCode();
        }

        public int Add(int amount)
        {
            return Set(Get() + amount);
        }

        public int Mul(int multiplier)
        {
            return Set(Get() * multiplier);
        }
    }
}