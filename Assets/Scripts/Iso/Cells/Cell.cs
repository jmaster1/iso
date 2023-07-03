using Iso.Buildings;

namespace Iso.Cells
{
    public class Cell
    {
        public int x, y;

        internal CellType cellType;
        
        public Building Building;

        public bool IsBuildable()
        {
            return cellType == CellType.Buildable && Building == null;
        }
        
        public bool IsTraversable()
        {
            return cellType is CellType.Buildable or CellType.Traversable && Building == null;
        }
    }
}