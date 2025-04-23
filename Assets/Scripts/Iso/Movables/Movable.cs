using System.Collections.Generic;
using Common.Util.Math;
using Iso.Cells;
using Iso.Util;
using Math;

namespace Iso.Movables
{
    public class Movable : AbstractManagedEntity<Movables, MovableEvent, Movable>, IBoundsProvider
    {
        private Cells.Cells Cells => Manager.Cells;
        
        public MovableInfo Info;

        /// <summary>
        /// currently occupied cell
        /// </summary>
        public Cell? Cell;

        /// <summary>
        /// current move source/target cells, null if not moving
        /// </summary>
        public Cell? cellFrom;

        /// <summary>
        /// current move source/target cells, null if not moving
        /// </summary>
        public Cell? cellTo;

        /// <summary>
        /// current path
        /// </summary>
        public readonly List<Cell?> Path = new();
        
        /**
		 * index of cellTo in a path, -1 if end of path reached
		 */
        public int cellToIndex = -1;

        /// <summary>
        /// actual position in the world
        /// </summary>
        public readonly Vector2DFloat pos = new();

        public float X => pos.X;
        
        public float Y => pos.Y;
        
        public bool IsCenteredInCell => X == Cell.CX && Y == Cell.CY;
        
        private void SetProperty<T>(ref T field, T value, MovableEvent eventType)
        {
	        if (EqualityComparer<T>.Default.Equals(field, value)) return;
	        field = value;
	        FireEvent(eventType);
        }
        
        /// <summary>
        /// heading direction (primary only: NESW)
        /// </summary>
        private Dir dir;
        
        public Dir Dir
        {
	        get => dir;
	        internal set => SetProperty(ref dir, value, MovableEvent.dirChange);
        }

        /// <summary>
        /// shows whether object is moving or not
        /// </summary>
        private bool moving;

        public bool Moving
        {
	        get => moving;
	        internal set => SetProperty(ref moving, value, MovableEvent.movingChange);
        }
        
        /// <summary>
        /// slected flag
        /// </summary>
        private bool selected;
        
        public bool Selected
        {
	        get => selected;
	        internal set => SetProperty(ref selected, value, MovableEvent.selectedChange);
        }
        
        /**
		 * move linear base velocity
		 */
        public float velocity;
	
        /**
		 * acceleration
		 */
        public float acceleration;
	
        /**
		 * current velocity
		 */
        private float speed;

        public bool MoveTo(Cell? target)
        {
	        if (target == null || target == Cell) return false;
	        var newPath = Cells.FindPath(Cell, target);
	        if (newPath == null) return false;
	        Assert(Cell == newPath[0]);
	        
	        Path.Clear();
		    Path.AddRange(newPath);
		    
		    cellFrom = Cell;
		    cellToIndex = 1;
		    cellTo = Path[1];
		    
		    var nextDir = cellFrom.DirectionTo(cellTo);
		    if (!IsCenteredInCell && !nextDir.IsSameOrInverted(dir))
		    {
			    cellFrom = Cell.FindSibling(dir);
			    Path.Insert(0, cellFrom);
			    cellTo = Cell;
			    nextDir = dir.Invert();
		    }
		    Dir = nextDir;
		    OnPathChange();
		    Moving = true;
		    return true;
        }

        private void OnPathChange()
        {
	        FireEvent(MovableEvent.pathChange);
        }

        public bool MoveTo(int tx, int ty)
        {
	        return MoveTo(Cells.Find(tx, ty));
        }

        public void update(float dt)
        {
	        //assert (int)pos.x == cell.getX();
			//assert (int)pos.y == cell.getY();
			if (!Moving) return;
			//
			// update speed from acceleration or velocity
			if(acceleration != 0) {
				if(speed != velocity) {
					var ds = acceleration * dt;
					speed += ds;
					if(speed > velocity) {
						speed = velocity;
					}
				}
			} else {
				speed = velocity;
			}
			//
			// update pos/cell
			//if (!isTeleporting()) {
			//    assert dir.isPrimary() : " dir " + dir + " cellTo " + cellTo + " obj.getUnitId " + obj.getUnitId();
			//}
			var hz = dir.IsHorz();
			float v = hz ? dir.X() : dir.Y();
			var lastCellPos = (int)(hz ? pos.x : pos.y);
			var togo = hz ? cellTo.CX - pos.X : cellTo.CY - pos.Y;
			var d = v * speed * Cell.GetVelocityMultiplier() * dt;
			//
			// check if finished cell-to-cell movement
			var remain = togo - d;
			var finished = System.Math.Sign(remain) != System.Math.Sign(v);
			if(finished) {
				pos.Set(cellTo.CX, cellTo.CY);
				Cell = cellTo;
				//
				// check end of path
				if(++cellToIndex == Path.Count) {
					cellFrom = null;
					cellTo = null;
					cellToIndex = -1;
					Path.Clear();
					OnPathChange();
					Moving = false;
					return;
				}

				//
				// get next cell
				cellFrom = cellTo;
				cellTo = Path[cellToIndex];
				Dir = cellFrom.DirectionTo(cellTo);
				//
				// update more by remain dt
				var remainDt = dt * System.Math.Abs(remain) / System.Math.Abs(d);
				update(remainDt);
			} else {
				//
				// advance pos
				if(hz) {
					pos.X += d;
				} else {
					pos.Y += d;
				}
				//
				// check cell changed
				var newCellPos = (int)(hz ? pos.x : pos.y);
				if(lastCellPos != newCellPos) {
					var x = hz ? newCellPos : Cell.X;
					var y = hz ? Cell.Y : newCellPos;
					var newCell = Cell.Get(x, y);
					Cell = newCell;
					FireEvent(MovableEvent.cellChange);
				}
			}
        }

        public void GetBounds(RectFloat target)
        {
	        target.Set(pos.x - Cell.HalfSize, pos.y - Cell.HalfSize, Cell.Size, Cell.Size);
        }

        public void Select()
        {
	        Manager.Select(this);
        }
        
        internal void SetSelected(bool val)
        {
	        Selected = val;
        }
    }
}
