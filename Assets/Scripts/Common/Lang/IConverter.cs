namespace Common.Lang
{
    public interface IConverter<TSource, TTarget>
    {
        TTarget Convert(TSource source);
    
        TSource Revert(TTarget target);
    }
}
