using System;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Iso.Buildings;

namespace Iso.Cells
{
    public class Cells : GenericBean
    {
        private int _width;
        private int _height;

        /// <summary>
        /// cell 2d map, may contain nulls
        /// </summary>
        private Cell[,] _cells;

        /// <summary>
        /// list of existing (non-null) cells
        /// </summary>
        public readonly PooledObsList<Cell> CellList = new();

        public int Width => _width;
        public int Heigth => _height;

        public void Create(int w, int h)
        {
            _cells = new Cell[_width = w, _height = h];
        }

        public Cell Get(int x, int y)
        {
            return _cells[x, y];
        }
        
        public Cell Find(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                return null;
            }
            return _cells[x, y];
        }

        public void Set(int x, int y, CellType type)
        {
            var cell = Find(x, y);
            if (cell == null)
            {
                _cells[x, y] = cell = CellList.Add(e =>
                {
                    e.x = x;
                    e.y = y;
                    e.cellType = type;
                });
            }
            else
            {
                cell.cellType = type;
            }
        }
        
        public void Clear(int x, int y)
        {
            var cell = Find(x, y);
            if (cell == null) return;
            CellList.PooledRemove(cell);
            _cells[x, y] = null;
        }

        public void ForEach(int x, int y, int w, int h, Action<Cell> action)
        {
            var x1 = x + w;
            var y1 = y + h;
            for (var xi = x; x <= x1; x++)
            {
                for (var yi = y; y <= y1; y++)
                {
                    action(Find(x, y));
                }
            }
        }

        public void ForEach(Cell cell, BuildingInfo info, bool flip, Action<Cell> action)
        {
            ForEach(cell.x, cell.y, flip ? info.height : info.width, flip ? info.width : info.height, action);
        }
    }
}
