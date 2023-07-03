namespace Common.Lang
{
    /// <summary>
    /// IObjectFactory counterpart - receives objects to be recycled 
    /// </summary>
    public interface IObjectReceiver
    {
        /// <summary>
        /// put object to recycle
        /// </summary>
        void Put(object obj);
    }
}