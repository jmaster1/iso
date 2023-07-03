namespace Common.View
{
    /// <summary>
    /// represents state of ViewInstance
    /// </summary>
    public enum ViewState
    {
        Undefined,
        
        /// <summary>
        /// view is scheduled for being shown, but not yet shown because of requested exclusive access
        /// </summary>
        Pending,
        
        Showing,
        Shown,
        Hiding,
        Hidden
    }
}