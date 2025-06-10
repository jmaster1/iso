using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Common.Lang.Observable;

namespace Iso.Buildings
{
    public class BuildingsConverter : NonAddingArrayConverter<PooledObsList<Building>, Building>
    {
    }
}
