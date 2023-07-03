using System;

namespace Common.View
{
    /// <summary>
    /// view object contract
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// invoked on back action from user
        /// </summary>
        /// <returns>true if processed</returns>
        bool OnBack();
        
        /// <summary>
        /// play show/hide animation and invoke callback on complete (if any),
        /// this method is invoked ViewManager in order to visualise show/hide ui transitions
        /// </summary>
        /// <param name="show">show or hide animation</param>
        /// <param name="onComplete">optional action that MUST be invoked on animation end</param>
        bool PlayAnimation(bool show, Action onComplete = null);

        /// <summary>
        /// assign viewInstance to this view
        /// </summary>
        ViewInstance ViewInstance { get; set; }
    }
}