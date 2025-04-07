using Common.Lang.Observable;
using Iso.Cells;
using Iso.Player;

namespace Iso.Buildings
{
    public class Buildings : AbstractIsoFeature<BuildingEvent, Building>
    {
        public Cells.Cells Cells;
        
        /// <summary>
        /// list of existing buildings
        /// </summary>
        public readonly PooledObsList<Building> List = new();
        
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
            return List.PooledAdd(building =>
            {
                building.Manager = this;
                building.Info = info;
                building.Flipped = flip;
                building.Cell = cell;
                Cells.ForEachCell(cell, info, flip,e => e.Building = building);
            });
        }

        public void Remove(Building building)
        {
            List.PooledRemove(building);
            building.ForEachCell(e => e.Building = null);
            building.Manager = default;
            building.Info = default;
            building.Flipped = default;
            building.Cell = default;
        }
    }
}
