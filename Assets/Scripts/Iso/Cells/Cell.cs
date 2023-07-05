using Common.Util.Math;
using Iso.Buildings;

namespace Iso.Cells
{
    public class Cell
    {
        internal Cells cells;
        
        internal int x, y;

        public int X => x;
        
        public int Y => y;

        public Cells Cells => cells;

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

        public Cell FindSibling(Dir dir)
        {
            return Cells.Find(x + dir.X(), y + dir.Y());
        }

        public override string ToString()
        {
            return "(" + x + ":" + y + ":" + cellType;
        }
    }
}