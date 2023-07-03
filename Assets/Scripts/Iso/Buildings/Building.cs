using System;
using Common.Lang.Entity;
using Iso.Cells;

namespace Iso.Buildings
{
    public class Building : AbstractEntity
    {
        public BuildingInfo Info;

        public Cell Cell;

        public bool Flipped;

        public void ForEachCell(Cells.Cells cells, Action<Cell> action)
        {
            cells.ForEach(Cell, Info, Flipped, action);
        }
    }
}