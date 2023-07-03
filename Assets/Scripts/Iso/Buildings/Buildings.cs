using Common.Lang.Entity;
using Common.Lang.Observable;
using Iso.Cells;

namespace Iso.Buildings
{
    public class Buildings : GenericBean
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
            Cells.ForEach(cell, info, flip, e => Validate(e.IsBuildable()));
            return List.Add(building =>
            {
                building.Info = info;
                building.Cell = cell;
                Cells.ForEach(cell, info, flip,e => e.Building = building);
            });
        }

        public void Remove(Building building)
        {
            List.PooledRemove(building);
            building.ForEachCell(Cells, e => e.Building = null);
        }
    }
}
