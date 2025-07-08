using Iso.Buildings;
using Iso.Cells;
using Iso.Player;

namespace Iso.Net.Common
{
    public class IsoWorldApi : IIsoWorldApi
    {
        private readonly IsoWorld _world;

        public IsoWorldApi(IsoWorld world)
        {
            _world = world;
        }

        public IsoWorld World => _world;

        public int Frame => _world.TimeGame.Frame;
    
        private Cell Cell(int x, int y) => World.Cells.Get(x, y);

        private BuildingInfo BuildingInfo(string id) => World.Buildings.BuildingInfoSet.GetById(id);

        public void Build(string id, int x, int y, bool flip = false) => 
            _world.Buildings.Build(BuildingInfo(id), Cell(x, y), flip);
    }
}