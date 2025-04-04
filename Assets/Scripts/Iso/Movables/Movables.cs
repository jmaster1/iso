using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.TimeNS;
using Common.Util.Math;
using Iso.Cells;

namespace Iso.Movables
{
    public class Movables : GenericBean
    {

        public Cells.Cells Cells;

        public Time Time;
        
        /// <summary>
        /// list of existing buildings
        /// </summary>
        public readonly PooledObsList<Movable> List = new();

        public void Start()
        {
            Time.AddListener(TimeListener);
        }

        private void TimeListener(Time obj)
        {
            var dt = (float)obj.Delta.TotalSeconds;
            foreach (var movable in List)
            {
                movable.update(dt);
            }
        }

        public Movable Add(MovableInfo info, Cell cell, Dir dir = Dir.N)
        {
            Validate(cell.IsTraversable());
            return List.PooledAdd(obj =>
            {
                obj.Movables = this;
                obj.Info = info;
                obj.Dir = dir;
                obj.Cell = cell;
                obj.Moving = false;
                obj.velocity = info.Velocity;
                obj.pos.Set(cell.CX, cell.CY);
            });
        }

        public void Remove(Movable obj)
        {
            List.PooledRemove(obj);
            obj.Movables = null;
            obj.Info = null;
        }

        public Events<MovableEvent, Movable> Events = new();
        
        internal void FireEvent(MovableEvent type, Movable movable)
        {
            Events.Fire(type, movable);
        }
    }
}
