using System;
using Common.Lang.Entity;
using Iso.Cells;

namespace Iso.Buildings
{
    public class Building : AbstractEntity
    {
        public Buildings buildings;

        Cells.Cells Cells => buildings.Cells;
        
        public BuildingInfo Info;

        public Cell Cell;

        public bool Flipped;

        public int X => Cell.X;
        
        public int Y => Cell.Y;

        public int Width => Flipped ? Info.height : Info.width;
        
        public int Height => Flipped ? Info.width : Info.height;

        public void ForEachCell(Action<Cell> action)
        {
            Cells.ForEachCell(Cell, Info, Flipped, action);
        }
    }
}