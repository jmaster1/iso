using Iso.Buildings;
using Iso.Cells;
using Iso.Player;

namespace IsoNet.Iso.Common;

public class IsoWorldApi(IsoWorld world) : IIsoWorldApi
{
    public IsoWorld World => world;

    public int Frame => world.TimeGame.Frame;
    
    private Cell Cell(int x, int y) => World.Cells.Get(x, y);

    private BuildingInfo BuildingInfo(string id) => World.Buildings.BuildingInfoSet.GetById(id);

    public void Build(string id, int x, int y, bool flip = false) => 
        world.Buildings.Build(BuildingInfo(id), Cell(x, y), flip);
}