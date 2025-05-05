using Common.TimeNS;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;

namespace IsoNet.Iso.Common;

public class IsoApi(IsoPlayer player, Time time) : IIsoApi
{
    public IsoPlayer Player => player;
    
    public void CreateCells(int width, int height)
    {
        Player.Cells.Create(width, height, () =>
        {
            Player.Cells.ForEachPos((x, y) => Player.Cells.Set(x, y, CellType.Buildable));    
        });
    }

    public void Start()
    {
        player.Bind(time);
    }

    public void Build(BuildingInfo buildingInfo, Cell cell, bool flip = false)
    {
        player.Buildings.Build(buildingInfo, cell, flip);
    }
}