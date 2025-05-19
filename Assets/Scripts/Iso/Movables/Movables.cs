using Common.Lang.Observable;
using Common.Util.Math;
using Iso.Cells;
using Iso.Player;

namespace Iso.Movables
{
    public class Movables : AbstractIsoFeature<MovableEvent, Movable>
    {
        public Cells.Cells Cells => World.Cells;
        
        /// <summary>
        /// list of existing buildings
        /// </summary>
        public readonly PooledObsList<Movable> List = new();

        public override void OnGameTimeUpdate(float dt)
        {
            foreach (var movable in List)
            {
                movable.update(dt);
            }
        }

        public Movable Add(MovableInfo info, Cell? cell, Dir dir = Dir.N)
        {
            Validate(cell.IsTraversable());
            return List.PooledAdd(obj =>
            {
                obj.Manager = this;
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
            obj.Manager = null;
            obj.Info = null;
        }

        public void Select(Movable movable)
        {
            if (movable is {Selected: true})
            {
                return;
            }

            foreach (var e in List)
            {
                if (e.Selected)
                {
                    e.SetSelected(false);
                }
            }
            movable.SetSelected(true);
        }

        public Movable FindSelected()
        {
            return List.FindFirst(e => e.Selected);
        }
    }
}
