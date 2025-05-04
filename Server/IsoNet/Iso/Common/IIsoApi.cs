using Iso.Buildings;
using Iso.Cells;

namespace IsoNet.Iso.Common;

public interface IIsoApi
{
    void CreateCells(int width, int height);
    
    void Start();
    
    void Build(BuildingInfo buildingInfo, Cell cell, bool flip = false);
}
