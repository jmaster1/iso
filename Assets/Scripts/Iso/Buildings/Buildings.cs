using Common.Lang.Observable;
using Iso.Cells;
using Iso.Player;

namespace Iso.Buildings
{
    public class Buildings : AbstractIsoFeature<BuildingEvent, Building>
    {
        public Cells.Cells Cells => World.Cells;
        
        /// <summary>
        /// list of existing buildings
        /// </summary>
        public readonly PooledObsList<Building> List = new();
        
        public bool IsBuildable(BuildingInfo info, Cell? cell, bool flip = false)
        {
            return cell != null && 
                   Cells.CheckBounds(cell.x, cell.y, 
                flip ? info.height : info.width, 
                flip ? info.width : info.height) &&
                Cells.ForEachPos(cell.x, cell.y, 
                flip ? info.height : info.width, 
                flip ? info.width : info.height, 
                (x, y) =>
            {
                var e = Cells.Find(x, y);
                return e != null && e.IsBuildable();
            });
        }

        public bool IsBuildable(BuildingInfo info, int x, int y, bool flip = false)
        {
            return IsBuildable(info, Cells.Find(x, y), flip);
        }

        /// <summary>
        /// build 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="cell"></param>
        /// <param name="flip"></param>
        /// <returns></returns>
        public Building Build(BuildingInfo info, Cell cell, bool flip = false)
        {
            Cells.ForEachCell(cell, info, flip, e => Validate(e.IsBuildable()));
            var building = List.PooledAdd(building =>
            {
                building.Manager = this;
                building.Info = info;
                building.Flipped = flip;
                building.Cell = cell;
                Cells.ForEachCell(cell, info, flip,e => e.Building = building);
            });
            FireEvent(BuildingEvent.BuildingCreated, building);
            return building;
        }

        public Building Build(BuildingInfo info, int x, int y, bool flip = false)
        {
            return Build(info, Cells.Get(x, y), flip);
        }

        public void Remove(Building building)
        {
            List.PooledRemove(building);
            building.ForEachCell(e => e.Building = null!);
            building.Manager = null!;
            building.Info = null!;
            building.Flipped = false;
            building.Cell = null!;
        }
    }
}
