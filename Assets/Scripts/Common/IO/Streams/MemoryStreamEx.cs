using System;
using System.IO;

namespace Common.IO.Streams
{
    /// <summary>
    /// MemoryStream extension
    /// </summary>
    public class MemoryStreamEx : MemoryStream
    {
        /// <summary>
        /// optional callback to invoke after dispose
        /// </summary>
        public Action DisposeAction;

        /// <summary>
        /// if true, will NOT call base.Dispose()
        /// </summary>
        public bool PreventDispose;
        
        protected override void Dispose(bool disposing)
        {
            if(!PreventDispose) base.Dispose(disposing);
            DisposeAction?.Invoke();
        }
    }
}