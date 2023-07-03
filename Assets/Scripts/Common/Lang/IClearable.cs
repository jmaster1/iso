namespace Common.Lang
{
    /// <summary>
    /// describes contract for bean that is capable to reset it's state.
    /// the difference between this and IDisposable is that
    /// IClearable might be reused after Clear(), but IDisposable - not after Dispose()
    /// </summary>
    public interface IClearable
    {
        /// <summary>
        /// clear bean state
        /// </summary>
        void Clear();
    }
}