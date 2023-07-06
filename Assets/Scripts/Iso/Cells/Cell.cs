using Common.Util.Math;
using Iso.Buildings;

namespace Iso.Cells
{
    public class Cell
    {
        private static readonly float Size = 1;

        private static readonly float HalfSize = Size / 2f;
        
        internal Cells cells;
        
        internal int x, y;

        public int X => x;
        
        public int Y => y;
        
        public float CX => x + HalfSize;
        
        public float CY => y + HalfSize;

        public Cells Cells => cells;

        public CellType cellType { get; internal set; }
        
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

        public float GetVelocityMultiplier()
        {
            return 1;
        }

        public Cell Get(int tx, int ty)
        {
            return Cells.Get(tx, ty);
        }

        public Dir DirectionTo(Cell target)
        {
            return DirEx.ValueOf(target.X - X, target.Y - Y);
        }

        public bool Is(int tx, int ty)
        {
            return x == tx && y == ty;
        }
    }
}
