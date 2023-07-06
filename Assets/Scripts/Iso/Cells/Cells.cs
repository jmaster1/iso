using System;
using System.Collections.Generic;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.Util.Math.Path.AStar;
using Iso.Buildings;

namespace Iso.Cells
{
    public class Cells : GenericBean
    {
        /// <summary>
        /// cell 2d map, may contain nulls
        /// </summary>
        private Cell[,] cells;

        /// <summary>
        /// list of existing (non-null) cells
        /// </summary>
        public readonly PooledObsList<Cell> CellList = new();
        
        private readonly AStarPathFinder<Cell> pathFinder = new();
        
        private readonly CellsGraph graph = new();

        public int Width { get; private set; }

        public int Heigth { get; private set; }
        
        public Events<CellEvent, Cell> Events = new();
        
        internal void FireEvent(CellEvent type, Cell cell)
        {
            Events.Fire(type, cell);
        }

        public void Create(int w, int h)
        {
            cells = new Cell[Width = w, Heigth = h];
        }

        public Cell Get(int x, int y)
        {
            return cells[x, y];
        }
        
        public Cell Find(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Heigth)
            {
                return null;
            }
            return cells[x, y];
        }

        public void Set(int x, int y, CellType type)
        {
            var cell = Find(x, y);
            if (cell == null)
            {
                cells[x, y] = cell = CellList.PooledAdd(e =>
                {
                    e.cells = this;
                    e.x = x;
                    e.y = y;
                    e.cellType = type;
                });
            }
            else
            {
                cell.cellType = type;
                FireEvent(CellEvent.cellTypeChange, cell);
            }
        }
        
        public void Clear(int x, int y)
        {
            var cell = Find(x, y);
            if (cell == null) return;
            CellList.PooledRemove(cell);
            cells[x, y] = null;
            cell.cells = null;
            cell.Building = null;
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
            ForEachCell(cell.x, cell.y, flip ? info.height : info.width, flip ? info.width : info.height, action);
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

        public void ForEachPos(Action<int, int> action)
        {
            ForEachPos(0, 0, Width, Heigth, action);
        }

        public List<Cell> FindPath(Cell from, Cell to)
        {
            return pathFinder.FindPath(graph, from, to);
        }
    }
}
