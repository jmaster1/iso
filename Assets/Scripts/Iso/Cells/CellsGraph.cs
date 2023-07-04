using Common.Util.Math;
using Common.Util.Math.Path;

namespace Iso.Cells
{
    public class CellsGraph : IGraph<Cell>
    {
        public int Distance(Cell from, Cell to)
        {
            if (from == null || to == null) return 0;
            var result = System.Math.Abs(from.X - to.X) + System.Math.Abs(from.Y - to.Y) * 3;
            /*if(from.isRoad() || from.isSidewalk()) {
                result--;
            }
            if(to.isRoad() || from.isSidewalk()) {
                result--;
            }*/
            return result;
        }

        public int GetSiblingCount(Cell c)
        {
            return DirEx.Primary.Length;
        }

        public Cell GetSibling(Cell c, int index)
        {
            var dir = DirEx.Primary[index];
            var sibling = c.FindSibling(dir);
            if (sibling != null && sibling.IsTraversable()) {
                return sibling;
            }
            return null;
        }
    }
}