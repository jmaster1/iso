using Iso.Buildings;
using Iso.Cells;

namespace IsoNet.Iso.Common;

public interface IIsoWorldApi
{
    void Build(BuildingInfo buildingInfo, Cell cell, bool flip = false);
}

public class ReplayAttribute : Attribute
{
}
