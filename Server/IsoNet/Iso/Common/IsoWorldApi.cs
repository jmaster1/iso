using Iso.Buildings;
using Iso.Cells;
using Iso.Player;

namespace IsoNet.Iso.Common;

public class IsoWorldApi(IsoWorld world) : IIsoWorldApi
{
    public IsoWorld World => world;
    
    /*
    public void CreateCells(int width, int height)
    {
        World.Cells.Create(width, height, () =>
        {
            World.Cells.ForEachPos((x, y) => World.Cells.Set(x, y, CellType.Buildable));    
        });
    }
    */

    public void Build(BuildingInfo buildingInfo, Cell cell, bool flip = false)
    {
        world.Buildings.Build(buildingInfo, cell, flip);
    }
}