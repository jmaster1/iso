using System;
using Common.Util.Math;
using Iso.Cells;
using Iso.Util;
using Newtonsoft.Json;

namespace Iso.Buildings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Building : AbstractManagedEntity<Buildings, BuildingEvent, Building>, IBoundsProvider
    {
        Cells.Cells Cells => Manager.Cells;
        
        [JsonProperty]
        public BuildingInfo Info;

        [JsonProperty]
        public Cell Cell;

        [JsonProperty]
        public bool Flipped;

        public int X => Cell.X;
        
        public int Y => Cell.Y;

        public int Width => Flipped ? Info.height : Info.width;
        
        public int Height => Flipped ? Info.width : Info.height;

        public void ForEachCell(Action<Cell> action)
        {
            Cells.ForEachCell(Cell, Info, Flipped, action);
        }
 
        public void GetBounds(RectFloat target)
        {
            target.Set(X, Y, Flipped ? Height : Width, Flipped ? Width : Height);
        }
    }
}