using Iso.Buildings;
using Iso.Cells;

namespace IsoNet.Iso.Common;

public interface IIsoClientApi
{
    void CreateCells(int width, int height);
    
    void Start();
    
    void Build(int frame, BuildingInfo buildingInfo, Cell cell, bool flip);
}
