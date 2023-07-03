namespace Common.TimeNS
{
    public enum TaskState
    {
        /// <summary>
        /// task is idle
        /// </summary>
        Idle,
        
        /// <summary>
        /// task is scheduled for execution
        /// </summary>
        Scheduled,
        
        /// <summary>
        /// task is paused
        /// </summary>
        Paused,
    
        /// <summary>
        /// task is being executed, set right before executing task action
        /// </summary>
        Running,
    
        /// <summary>
        /// task execution completed, set right after task action execution
        /// </summary>
        Finished,
    
        /// <summary>
        /// task cancelled
        /// </summary>
        Cancelled,
    }
}