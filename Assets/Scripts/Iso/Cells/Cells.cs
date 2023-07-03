using Common.Lang.Entity;

namespace Iso.Cells
{
    public class Cells : GenericBean
    {
        private int _width;
        private int _height;

        private Cell[,] _cells;

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
        
        public Cell GetSafe(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                return null;
            }
            return _cells[x, y];
        }
        
    }
}
