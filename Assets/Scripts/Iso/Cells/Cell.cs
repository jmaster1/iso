using Common.Util.Math;
using Iso.Buildings;
using Iso.Util;

namespace Iso.Cells
{
    public class Cell : AbstractManagedEntity<Cells, CellEvent, Cell>
    {
        public const float Size = 1;

        public const float HalfSize = Size / 2f;
        
        internal Cells cells;
        
        internal int x, y;

        public int X => x;
        
        public int Y => y;
        
        public float Cx => x + HalfSize;
        
        public float Cy => y + HalfSize;

        public Cells Cells => cells;

        public CellType CellType { get; internal set; }
        
        public Building? Building;

        public bool IsBuildable()
        {
            return CellType == CellType.Buildable && Building == null;
        }
        
        public bool IsTraversable()
        {
            return CellType is CellType.Buildable or CellType.Traversable && Building == null;
        }

        public Cell? FindSibling(Dir dir)
        {
            return Cells.Find(x + dir.X(), y + dir.Y());
        }

        public override string ToString()
        {
            return "(" + x + ":" + y + ":" + CellType;
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

        public Cell? Set(CellType type)
        {
            return cells.Set(this, type);
        }
    }
}
