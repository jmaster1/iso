using Common.Lang;
using Common.Lang.Observable;

namespace Common.Bind
{
    /// <summary>
    /// bindable is activation pattern for bean (bindable) that depends on another bean (model).
    /// general usage of this is view-model dependency
    /// </summary>
    public interface IBindable<T>
    {
        /// <summary>
        /// bind to model, if binding bean that is already bound,
        /// this will be unbound first.
        /// must not be invoked in temporal state
        /// </summary>
        void Bind(T model);

        /// <summary>
        /// current model retrieval, null when not bound
        /// </summary>
        /// <returns></returns>
        T GetModel();

        /// <summary>
        /// model holder retrieval, never null
        /// </summary>
        /// <returns></returns>
        Holder<T> GetModelHolder();

        /// <summary>
        /// current state of bindable retrieval,
        /// there are 2 temporal states (binding/unbinding) that valid while [un]bind() invocation is in progress
        /// </summary>
        /// <returns></returns>
        BindableState GetBindableState();

        /// <summary>
        /// unbind from model, this method might be invoked when Bound/Unbound,
        /// but not in temporal state
        /// </summary>
        void Unbind();
        
        /// <summary>
        /// check if is bound
        /// </summary>
        /// <returns>true if state == bound</returns>
        bool IsBound();
    }
}
