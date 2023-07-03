namespace Common.Api.Local
{
    /// <summary>
    /// shows how to behave on message retrieval by key
    /// </summary>
    public enum MessageResolutionPolicy
    {
        /// <summary>
        /// return value or null if not found (default production policy)
        /// </summary>
        ValueOrNull,
        
        /// <summary>
        /// return value or key if not found (default debug policy)
        /// </summary>
        ValueOrKey,
        
        /// <summary>
        /// return value or throw Exception if not found (validation purposes)
        /// </summary>
        ValueOrError,
        
        /// <summary>
        /// return key (debug purposes)
        /// </summary>
        Key
    }
}