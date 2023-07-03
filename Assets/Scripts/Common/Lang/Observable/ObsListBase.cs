using Common.Lang.Entity;

namespace Common.Lang.Observable
{
    public abstract class ObsListBase : AbstractEntityIdString
    {
        public abstract int GetCount();

        public abstract object GetElement(int index);
    }
}