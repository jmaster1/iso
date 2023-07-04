using System;

namespace Common.Util.Math
{
	public class Vector2DFloat
	{
		public float X { get; private set; }

		public float Y { get; private set; }

		public Vector2DFloat()
		{
		}

		public Vector2DFloat(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		public Vector2DFloat Set(Vector2DFloat pt)
		{
			if (pt == null)
			{
				X = Y = 0;
			}
			else
			{
				X = pt.X;
				Y = pt.Y;
			}

			return this;
		}

		public Vector2DFloat Set(float x1, float y1)
		{
			X = x1;
			Y = y1;
			return this;
		}

		public override string ToString()
		{
			return "(" + X + ", " + Y + ")";
		}

		/**
		 * multiply components
		 */
		public void Mul(float scale)
		{
			X *= scale;
			Y *= scale;
		}

		/**
		 * divide components by specified point
		 */
		public void Div(Vector2DFloat pt)
		{
			X /= pt.X;
			Y /= pt.Y;
		}

		public void Reset()
		{
			X = Y = 0;
		}

		/**
		 * retrieve distance (>= 0) to specified point
		 */
		public float Distance(Vector2DFloat pt)
		{
			var dx = X - pt.X;
			var dy = Y - pt.Y;
			return (float)System.Math.Sqrt(dx * dx + dy * dy);
		}

		public float Distance(float x2, float y2)
		{
			var dx = X - x2;
			var dy = Y - y2;
			return (float)System.Math.Sqrt(dx * dx + dy * dy);
		}

		public float Distance2(float x2, float y2)
		{
			var dx = X - x2;
			var dy = Y - y2;
			return dx * dx + dy * dy;
		}

		/**
		 * create array of points of specified size
		 */
		public static Vector2DFloat[] CreatePoints(int n)
		{
			var result = new Vector2DFloat[n];
			for (var i = 0; i < n; i++)
			{
				result[i] = new Vector2DFloat();
			}

			return result;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector2DFloat o2)
			{
				return Equals(obj);
			}

			return false;
		}

		protected bool Equals(Vector2DFloat other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}

		public void MoveBy(float dx, float dy)
		{
			X += dx;
			Y += dy;
		}

		public float Len()
		{
			return (float)System.Math.Sqrt(X * X + Y * Y);
		}

		public bool IsZero()
		{
			return X == 0 && Y == 0;
		}

		public Vector2DFloat Sub(Vector2DFloat pt)
		{
			X -= pt.X;
			Y -= pt.Y;
			return this;
		}


		/**
		 * set this points coordinate from base and vector specified by angle/len
		 */
		public void Set(Vector2DFloat bas, float angle, float len)
		{
			X = (float)(bas.X + len * System.Math.Cos(angle));
			Y = (float)(bas.Y + len * System.Math.Sin(angle));
		}

		/**
		 * move this point towards specified using scale
		 * @param target a target point for moving direction
		 * @param scale multiplier used to calculate move path length (scale * len(this, target))
		 */
		public void MoveTo(Vector2DFloat target, float scale)
		{
			X += (target.X - X) * scale;
			Y += (target.Y - Y) * scale;
		}

		public void SetNaN()
		{
			X = Y = float.NaN;
		}

		public bool IsNaN()
		{
			return float.IsNaN(X) || float.IsNaN(Y);
		}

		/**
		 * Angle in degress between this and other vector
		 */
		public float Angle(float x, float y)
		{
			return (float)System.Math.Atan2(this.X * y - this.Y * x, this.X * x + this.Y * y);
		}
	}
}