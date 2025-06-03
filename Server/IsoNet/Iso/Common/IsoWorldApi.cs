using Iso.Buildings;
using Iso.Cells;
using Iso.Player;

namespace IsoNet.Iso.Common;

public class IsoWorldApi(IsoWorld world) : IIsoWorldApi
{
    public IsoWorld World => world;
    
    private Cell Cell(int x, int y) => World.Cells.Get(x, y);

    private BuildingInfo BuildingInfo(string id) => World.Buildings.BuildingInfoSet.GetById(id);
    
    /*
    public void CreateCells(int width, int height)
    {
        World.Cells.Create(width, height, () =>
        {
            World.Cells.ForEachPos((x, y) => World.Cells.Set(x, y, CellType.Buildable));    
        });
    }
    */

    public void Build(string id, int x, int y, bool flip = false) => 
        world.Buildings.Build(BuildingInfo(id), Cell(x, y), flip);
}