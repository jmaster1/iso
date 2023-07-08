using Iso.Cells;
using UnityEngine;

namespace Iso.Unity.World
{
    public static class IsoUnity
    {
        public static Cell Find(this Cells.Cells cells, Vector2 v)
        {
            return cells.Find((int)v.x, (int)v.y);
        }
        
        public static Cell Find(this Cells.Cells cells, Vector2Int v)
        {
            return cells.Find(v.x, v.y);
        }
        
        public static Cell Set(this Cells.Cells cells, Vector2Int v, CellType type)
        {
            return cells.Set(v.x, v.y, type);
        }
    }
}