using System;
using Common.TimeNS;
using Iso.Player;

namespace Iso.Tests
{
    public class AbstractPlayerTest
    {
        public readonly IsoPlayer Player = new();
        public Cells.Cells Cells => Player.Cells;
        public Buildings.Buildings Buildings => Player.Buildings;
        public Movables.Movables Movables => Player.Movables;
        
        protected void UpdateTime(float seconds)
        {
            var t = new Time();
            Player.Bind(t);
            
            var span = TimeSpan.FromSeconds(seconds);
            var dt = TimeSpan.FromMilliseconds(50);
            while (t.Value - DateTime.MinValue < span)
            {
                t.Update(dt);
            }
        }
    }
}
