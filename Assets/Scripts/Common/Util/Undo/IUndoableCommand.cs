using System;

namespace Common.Util.Undo
{
    /// <summary>
    /// command that might be applied/reverted
    /// </summary>
    public interface IUndoableCommand
    {
        /// <summary>
        /// apply command, invoke onComplete when done
        /// </summary>
        void Apply(Action onEnd);

        /// <summary>
        /// revert command, invoke onComplete when done
        /// </summary>
        void Revert(Action onComplete);
    }
}