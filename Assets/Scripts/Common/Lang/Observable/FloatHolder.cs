namespace Common.Lang.Observable
{
    public class FloatHolder : Holder<float>
    {
        public float Add(float amount)
        {
            return Set(Get() + amount);
        }
    }
}