using Common.Bind;

namespace Common.Unity.Bind
{
    /// <summary>
    /// BindableBean to be used by BindableMono
    /// (because BindableMono can't extend BindableBean)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class BindableForMono<T> : BindableBean<T>
    {

        public BindableMono<T> BindableMono;
        
        public BindableForMono(BindableMono<T> bindableMono)
        {
            BindableMono = bindableMono;
        }

        protected override void OnBind()
        {
            BindableMono.OnBind();
        }
        
        protected override void OnUnbind()
        {
            BindableMono.OnUnbind();
        }
    }
}