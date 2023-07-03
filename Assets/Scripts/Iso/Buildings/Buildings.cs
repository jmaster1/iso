using Common.Api.Pool;
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
        public readonly ObsList<Building> List = new();

        private Pool<Building> pool = new();
        
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
            var building = pool.Get();
            building.Info = info;
            building.Cell = cell;
            Cells.ForEach(cell, info, flip,e => e.Building = building);
            List.Add(building);
            return building;
        }

        public void Remove(Building building)
        {
            Validate(List.Contains(building));
            List.Remove(building);
            building.ForEachCell(Cells, e => e.Building = null);
            pool.Put(building);
        }
    }
}
