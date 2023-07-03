using Common.Lang;

namespace Common.Util.Reflect
{
    /// <summary>
    /// IObjectFactory reflection based implementation
    /// </summary>
    public class ReflectionObjectFactory : IObjectFactory
    {
        public T Create<T>() where T: class
        {
            return ReflectHelper.NewInstance<T>();
        }
    }
}