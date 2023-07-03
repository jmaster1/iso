using System;

namespace Common.Util.Math
{
	public class PointFloat
	{
		private float _x, _y;

		public float X => _x;
		
		public float x => _x;

		public float Y => _y;
		
		public float y => _y;

		public PointFloat()
		{
		}

		public PointFloat(float x, float y)
		{
			this._x = x;
			this._y = y;
		}

		public PointFloat Set(PointFloat pt)
		{
			if (pt == null)
			{
				_x = _y = 0;
			}
			else
			{
				_x = pt._x;
				_y = pt._y;
			}

			return this;
		}

		public PointFloat Set(float x1, float y1)
		{
			_x = x1;
			_y = y1;
			return this;
		}

		public override string ToString()
		{
			return "(" + _x + ", " + _y + ")";
		}

		public float GetX()
		{
			return _x;
		}

		public void SetX(float x)
		{
			this._x = x;
		}

		public float GetY()
		{
			return _y;
		}

		public void SetY(float y)
		{
			this._y = y;
		}

		/**
		 * multiply components
		 */
		public void Mul(float scale)
		{
			_x *= scale;
			_y *= scale;
		}

		/**
		 * divide components by specified point
		 */
		public void Div(PointFloat pt)
		{
			_x /= pt._x;
			_y /= pt._y;
		}

		public void Reset()
		{
			_x = _y = 0;
		}

		/**
		 * retrieve distance (>= 0) to specified point
		 */
		public float Distance(PointFloat pt)
		{
			var dx = _x - pt._x;
			var dy = _y - pt._y;
			return (float)System.Math.Sqrt(dx * dx + dy * dy);
		}

		public float Distance(float x2, float y2)
		{
			var dx = _x - x2;
			var dy = _y - y2;
			return (float)System.Math.Sqrt(dx * dx + dy * dy);
		}

		public float Distance2(float x2, float y2)
		{
			var dx = _x - x2;
			var dy = _y - y2;
			return dx * dx + dy * dy;
		}

		/**
		 * create array of points of specified size
		 */
		public static PointFloat[] CreatePoints(int n)
		{
			PointFloat[] result = new PointFloat[n];
			for (int i = 0; i < n; i++)
			{
				result[i] = new PointFloat();
			}

			return result;
		}

		public override bool Equals(object obj)
		{
			if (obj is PointFloat o2)
			{
				return Equals(obj);
			}

			return false;
		}

		protected bool Equals(PointFloat other)
		{
			return _x.Equals(other._x) && _y.Equals(other._y);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_x, _y);
		}

		public void MoveBy(float dx, float dy)
		{
			_x += dx;
			_y += dy;
		}

		public float Len()
		{
			return (float)System.Math.Sqrt(_x * _x + _y * _y);
		}

		public bool IsZero()
		{
			return _x == 0 && _y == 0;
		}

		public PointFloat Sub(PointFloat pt)
		{
			_x -= pt._x;
			_y -= pt._y;
			return this;
		}


		/**
		 * set this points coordinate from base and vector specified by angle/len
		 */
		public void Set(PointFloat bas, float angle, float len)
		{
			_x = (float)(bas._x + len * System.Math.Cos(angle));
			_y = (float)(bas._y + len * System.Math.Sin(angle));
		}

		/**
		 * move this point towards specified using scale
		 * @param target a target point for moving direction
		 * @param scale multiplier used to calculate move path length (scale * len(this, target))
		 */
		public void MoveTo(PointFloat target, float scale)
		{
			_x += (target._x - _x) * scale;
			_y += (target._y - _y) * scale;
		}

		public void SetNaN()
		{
			_x = _y = float.NaN;
		}

		public bool IsNaN()
		{
			return float.IsNaN(_x) || float.IsNaN(_y);
		}

		/**
		 * Angle in degress between this and other vector
		 */
		public float Angle(float x, float y)
		{
			return (float)System.Math.Atan2(_x * y - _y * x, _x * x + _y * y);
		}
	}
}