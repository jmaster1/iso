using Common.Lang.Entity;

namespace Iso.Util
{

    public class AbstractManagedEntity<TManager> : AbstractEntity
    {
        public TManager Manager;
    }

}