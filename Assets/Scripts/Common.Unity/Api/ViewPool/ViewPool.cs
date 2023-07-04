using System;
using Common.Api.Pool;
using Common.Unity.Bind;

namespace Common.Unity.Api.ViewPool
{
    public class ViewPool : Pool<BindableMonoRaw>
    {
        public readonly string ViewId;

        public readonly BindableMonoRaw Prefab;
        
        public ViewPool(string viewId, BindableMonoRaw prefab,
            Func<BindableMonoRaw> factory) : base(typeof(BindableMonoRaw), factory)
        {
            ViewId = viewId;
            Prefab = prefab;
        }
    }
}