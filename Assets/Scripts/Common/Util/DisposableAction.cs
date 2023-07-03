using System;

namespace Common.Util
{
    /// <summary>
    /// IDisposable implementation that invokes action on dispose
    /// </summary>
    public class DisposableAction : IDisposable
    {
        private readonly Action action;

        public DisposableAction(Action action)
        {
            LangHelper.Validate(action != null);
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}