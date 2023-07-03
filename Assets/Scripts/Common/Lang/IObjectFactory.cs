namespace Common.Lang
{
    /// <summary>
    /// api capable of creating/destroying objects
    /// </summary>
    public interface IObjectFactory
    {
     
        /// <summary>
        /// create object of specified type
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        T Create<T>() where T : class;
    }
}