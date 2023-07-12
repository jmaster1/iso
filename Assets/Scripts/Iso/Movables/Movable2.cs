using System.Collections.Generic;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.Util.Math;
using Iso.Cells;
using Iso.Util;

namespace Iso.Movables
{
    public class Movable2 : AbstractEntity
    {
        public Movables Movables;

        public Events<MovableEvent, Movable> Events => Movables.Events;

        Cells.Cells Cells => Movables.Cells;
        
        public MovableInfo Info;

        /// <summary>
        /// currently occupied cell (if any)
        /// </summary>
        public Cell cell;

        /// <summary>
        /// actual position in the world
        /// </summary>
        public readonly Vector2DFloat pos = new();
        
        /// <summary>
        /// target position in the world
        /// </summary>
        public readonly Vector2DFloat posTarget = new();
        
        /// <summary>
        /// current speed
        /// </summary>
        public readonly Vector2DFloat speed = new();
        
        /**
		 * move linear base velocity
		 */
        public float speedTarget;
        
        /**
		 * acceleration
		 */
        public float acceleration;

        public float X => pos.X;
        
        public float Y => pos.Y;

        /// <summary>
        /// shows whether object is moving or not
        /// </summary>
        private bool moving;
        
        readonly Vector2DFloat tmp = new();

        public bool Moving
        {
	        get => moving;
	        internal set
	        {
		        if (moving == value) return;
		        moving = value;
		        //FireEvent(MovableEvent.movingChange);
	        }
        }
        
        public void update(float dt)
        {
			if (!Moving) return;
			//
			// update speed
			var speedLen = speed.Len();
			if(!speedTarget.FloatEquals(speedLen)) {
				var ds = acceleration * dt;
				speedLen += ds;
				if (speedLen > speedTarget)
				{
					speedLen = speedTarget;
				}

				speed.Set(posTarget).Sub(pos).SetLen(speedLen);
			}
			//
			// update pos
			
			//
			// update pos/cell
			//if (!isTeleporting()) {
			//    assert dir.isPrimary() : " dir " + dir + " cellTo " + cellTo + " obj.getUnitId " + obj.getUnitId();
			//}
			
			/*
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
			*/
        }

        private void FireEvent(MovableEvent type)
        {
	        //Movables?.FireEvent(type, this);
        }
    }
}
