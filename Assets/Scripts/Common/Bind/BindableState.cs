namespace Common.Bind
{
 
    /// <summary>
    /// represents state of IBindable
    /// </summary>
    public enum BindableState
    {
        Unbound,
        Binding,
        Bound,
        Unbinding
    }
    
    public static class Extensions 
    {
        /// <summary>
        /// check if BindableState is temporal
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsTemporal(this BindableState e)
        {
            switch (e)
            {
                case BindableState.Binding:
                case BindableState.Unbinding:
                    return true;
            }
            return false;
        }
    }
}