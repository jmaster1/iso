using System;
using System.Collections.Generic;
using Common.Lang.Observable;
using Common.Util.Math.Path.AStar;
using Iso.Buildings;
using Iso.Player;
using Math;

namespace Iso.Cells
{
    public class Cells : AbstractIsoFeature<CellEvent, Cell>
    {
        /// <summary>
        /// cell 2d map, may contain nulls
        /// </summary>
        private Cell?[,]? _cells;

        /// <summary>
        /// list of existing (non-null) cells
        /// </summary>
        public readonly PooledObsList<Cell> CellList = new();
        
        private readonly AStarPathFinder<Cell> _pathFinder = new();
        
        private readonly CellsGraph _graph = new();

        public int Width { get; private set; }

        public int Heigth { get; private set; }

        public void Create(int w, int h, Action? init = null)
        {
            Clear();
            _cells = new Cell[Width = w, Heigth = h];
            init?.Invoke();
            FireEvent(CellEvent.CellsCreated);
        }

        public override void Clear()
        {
            base.Clear();
            CellList.Clear();
            _cells = null;
            Width = Heigth = 0;
        }

        public Cell Get(int x, int y)
        {
            return _cells![x, y]!;
        }
        
        public Cell Get(float x, float y)
        {
            return _cells![(int)x, (int)y]!;
        }
        
        public Cell? Find(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Heigth)
            {
                return null;
            }
            return _cells![x, y];
        }

        public Cell? Find(float x, float y)
        {
            return Find((int) x, (int) y);
        }
        
        public Cell? Set(int x, int y, CellType type)
        {
            var cell = Find(x, y);
            if (cell != null) return Set(cell, type);
            return _cells![x, y] = CellList.PooledAdd(e =>
            {
                e.cells = this;
                e.x = x;
                e.y = y;
                e.cellType = type;
            });
        }
        
        public Cell? Set(Cell? cell, CellType type)
        {
            cell.cellType = type;
            FireEvent(CellEvent.CellTypeChange, cell);
            return cell;
        }
        
        public void Clear(int x, int y)
        {
            var cell = Find(x, y);
            if (cell == null) return;
            CellList.PooledRemove(cell);
            _cells![x, y] = null;
            cell.cells = null!;
            cell.Building = null!;
            cell.cellType = default;
            cell.x = cell.y = 0;
        }

        public void ForEachCell(int x, int y, int w, int h, Action<Cell> action)
        {
            var x1 = x + w;
            var y1 = y + h;
            for (var xi = x; xi < x1; xi++)
            {
                for (var yi = y; yi < y1; yi++)
                {
                    var cell = Find(xi, yi);
                    if (cell != null)
                    {
                        action(cell);
                    }
                }
            }
        }

        public void ForEachCell(Cell cell, BuildingInfo info, bool flip, Action<Cell> action)
        {
            ForEachCell(cell.x, cell.y, 
                flip ? info.height : info.width, 
                flip ? info.width : info.height,
                action);
        }
        
        public void ForEachPos(int x, int y, int w, int h, Action<int, int> action)
        {
            var x1 = x + w;
            var y1 = y + h;
            for (var xi = x; xi < x1; xi++)
            {
                for (var yi = y; yi < y1; yi++)
                {
                    action(xi, yi);
                }
            }
        }
        
        public bool ForEachPos(int x, int y, int w, int h, Func<int, int, bool> action)
        {
            var x1 = x + w;
            var y1 = y + h;
            for (var xi = x; xi < x1; xi++)
            {
                for (var yi = y; yi < y1; yi++)
                {
                    if (!action(xi, yi))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void ForEachPos(Action<int, int> action)
        {
            ForEachPos(0, 0, Width, Heigth, action);
        }

        public List<Cell> FindPath(Cell from, Cell to)
        {
            return _pathFinder.FindPath(_graph, from, to);
        }

        public void ForEachPos(RectFloat region, Action<int, int> action)
        {
            ForEachPos((int)region.x, (int)region.y, (int)region.w, (int)region.h, action);
        }

        public bool CheckBounds(int x, int y, int w, int h)
        {
            return x >= 0 && y >= 0 && x + w <= Width && y + h <= Heigth;
        }
    }
}
