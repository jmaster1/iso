using System.Collections.Generic;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.Util.Math;
using Iso.Cells;

namespace Iso.Movables
{
    public class Movable : AbstractEntity
    {
        public Movables Movables;

        public Events<MovableEvent, Movable> Events => Movables.Events;

        Cells.Cells Cells => Movables.Cells;
        
        public MovableInfo Info;

        /// <summary>
        /// currently occupied cell
        /// </summary>
        public Cell cell;
        
        /// <summary>
        /// current move source/target cells
        /// </summary>
        public Cell cellFrom, cellTo;

        /// <summary>
        /// current path
        /// </summary>
        public readonly List<Cell> path = new();
        
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

        /// <summary>
        /// heading direction (primary only: NESW)
        /// </summary>
        private Dir dir;
        
        public Dir Dir
        {
	        get => dir;
	        internal set
	        {
		        if (dir == value) return;
		        dir = value;
		        FireEvent(MovableEvent.dirChange);
	        }
        }

        /// <summary>
        /// shows whether object is moving or not
        /// </summary>
        private bool moving;

        public bool Moving
        {
	        get => moving;
	        internal set
	        {
		        if (moving == value) return;
		        moving = value;
		        FireEvent(MovableEvent.movingChange);
	        }
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

        public bool MoveTo(Cell target)
        {
	        if (target == null) return false;
	        var newPath = Cells.FindPath(cell, target);
	        if (newPath == null) return false;
	        path.Clear();
		    path.AddRange(newPath);
		    cellFrom = cell;
		    cellToIndex = 1;
		    cellTo = path[1];
		    Dir = cellFrom.DirectionTo(cellTo);
		    Moving = true;
		    return true;
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
			var d = v * speed * cell.GetVelocityMultiplier() * dt;
			//assert togo == 0 || Math.signum(togo) == Math.signum(d);
			//
			// check if finished cell-to-cell movement
			var remain = togo - d;
			var finished = System.Math.Sign(remain) != System.Math.Sign(v);
			if(finished) {
				pos.Set(cellTo.CX, cellTo.CY);
				//obj.bounds.moveCenterTo(pos);
				//obj.viewBounds.reset();
				cell = cellTo;
				//
				// check end of path
				if(++cellToIndex == path.Count) {
					cellFrom = null;
					cellTo = null;
					cellToIndex = -1;
					path.Clear();
					Moving = false;
					FireEvent(MovableEvent.pathEnd);
					return;
				}

				//
				// get next cell
				cellFrom = cellTo;
				cellTo = path[cellToIndex];
				Dir = cellFrom.DirectionTo(cellTo);
				//
				// update more by remain dt
				var remainDt = dt * System.Math.Abs(remain) / System.Math.Abs(d);
				//assert remainDt >= 0;
				//assert remainDt <= dt;
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
					var x = hz ? newCellPos : cell.X;
					var y = hz ? cell.Y : newCellPos;
					var newCell = cell.Get(x, y);
					//assert newCell != cell;
					cell = newCell;
					FireEvent(MovableEvent.cellChange);
				}
			}
        }

        private void FireEvent(MovableEvent type)
        {
	        Movables?.FireEvent(type, this);
        }
    }
}
