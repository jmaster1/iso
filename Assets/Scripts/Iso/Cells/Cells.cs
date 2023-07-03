using Common.Lang.Entity;
using Common.Lang.Observable;

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
        public readonly ObsList<Cell> CellList = new();
        
        public Events<CellsEvent, Cell> Events = new();

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
                _cells[x, y] = cell = new Cell()
                {
                    x = x,
                    y = y,
                    cellType = type
                };
                CellList.Add(cell);
                Events.Fire(CellsEvent.CellAdded, cell);
            }
            else
            {
                cell.cellType = type;
                Events.Fire(CellsEvent.CellTypeChanged, cell);
            }
        }
        
        public void Clear(int x, int y)
        {
            var cell = Find(x, y);
            if (cell == null) return;
            Events.Fire(CellsEvent.CellRemoved, cell);
            CellList.Remove(cell);
            _cells[x, y] = null;
        }
    }
}
