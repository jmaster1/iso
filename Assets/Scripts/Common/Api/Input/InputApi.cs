using System;
using Common.IO.Streams;
using Common.Lang.Observable;
using Common.Util;
using Common.Util.Http;

namespace Common.Api.Input
{
    /// <summary>
    /// user input management api
    /// </summary>
    public class InputApi : AbstractApi
    {
        /// <summary>
        /// global input lock
        /// </summary>
        public readonly BoolHolderLock Lock = new();

        public IDisposable InputLockAdd(object val)
        {
            Lock.AddLock(val);
            return LangHelper.CreateDisposable(() => InputLockRemove(val));
        }
        
        public void InputLockRemove(object val)
        {
            Lock.RemoveLock(val);
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.h3("Lock");
            html.tableHeader("#", "lock");
            foreach (var e in Lock.Locks)
            {
                html.tr().tdRowNum().td(e).endTr();
            }
            html.endTable();
        }
    }
}