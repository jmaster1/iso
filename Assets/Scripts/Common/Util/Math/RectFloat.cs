using System;
using Common.Util;
using Common.Util.Math;

/**
 * RectFloat rectangle with float type components
 * 
 * @author timur
 */

namespace Math {
	public class RectFloat : Shape2D {
		public const int PT_LEFT_BOTTOM = 0;
		public const int PT_RIGHT_BOTTOM = 1;
		public const int PT_RIGHT_TOP = 2;
		public const int PT_LEFT_TOP = 3;

		/**
		 * initialize array of 4 points that match rectangle corners
		 * @return array of points, use PT_?_? constants to access points
		 */
		public static Vector2DFloat[] GetRectPoints(float x, float y, float w, float h, Vector2DFloat[] target) {
			if(target == null) {
				target = Vector2DFloat.CreatePoints(4);
			}
			target[PT_LEFT_BOTTOM].Set(x, y);
			target[PT_RIGHT_BOTTOM].Set(x + w, y);
			target[PT_RIGHT_TOP].Set(x + w, y + h);
			target[PT_LEFT_TOP].Set(x, y + h);
			return target;
		}

	    public float x;
	    
	    public float y;
	    
	    public float w;
	    
	    public float h;
		
	    //private SegmentFloat _segment;

	    /**
	     * default constructor
	     */
	    public RectFloat() {
	    }

	    /**
	     * constructor with components
	     * 
	     * @param x0
	     * @param y0
	     */
	    public RectFloat(float x0, float y0, float w0, float h0) {
	        x = x0;
	        y = y0;
	        w = w0;
	        h = h0;
	    }

	    /**
	     * constructor with 2 points defining diagonal
	     */
	    public RectFloat(Vector2DFloat pt0, Vector2DFloat pt1) {
    		SetDiagonal(pt0.X, pt0.Y, pt1.X, pt1.Y);
		}

	    /**
	     * copy constructor
	     */
		public RectFloat(RectFloat rc) {
			Set(rc);
		}

	    /**
	     * check if rectangle contains specified point
	     * @return true if point is inside or on the edge
	     */
	    public bool Contains(float px, float py) {
	        return x <= px && y <= py && x + w >= px && y + h >= py;
	    }

		public override string ToString() {
	        return "(" + x + ", " + y + ", " + w + ", " + h + ")";
	    }

	    /**
	     * expand rectangle (if necessary) so specified point is inside or on edge
	     * 
	     * @param x2
	     * @param y2
	     * @return true if rectangle modified
	     */
	    public bool Add(float x2, float y2) {
	        var result = false;
	        if (x > x2) {
	            w += x - x2;
	            x = x2;
	            result = true;
	        } else if (x + w < x2) {
	            w = x2 - x;
	            result = true;
	        }
	        if (y > y2) {
	            h += y - y2;
	            y = y2;
	            result = true;
	        } else if (y + h < y2) {
	            h = y2 - y;
	            result = true;
	        }
	        return result;
	    }
	    
	    public bool Add(Vector2DFloat pt) {
	        return Add(pt.X, pt.Y);
	    }

	    /**
	     * expand rectangle (if necessary) so specified rectangle contained by this
	     * 
	     * @param bounds
	     */
	    public bool Add(RectFloat bounds) {
	        return Add(bounds.x, bounds.y) |
	                Add(bounds.x + bounds.w, bounds.y + bounds.h);
	    }
	    
	    /**
	     * expand rectangle (if necessary) so this contains specified circle
	     * @param x x-component of circle center
	     * @param y y-component of circle center
	     * @param r circle radius
	     */
	    public bool Add(float tx, float ty, float r) {
	        return Add(tx - r, ty - r) | Add(tx + r, ty + r);
	    }

	    /**
	     * set rectangle components
	     * 
	     * @param x
	     * @param y
	     * @param w
	     * @param h
	     * @return 
	     */
	    public RectFloat Set(float tx, float ty, float tw, float th) {
	        x = tx;
	        y = ty;
	        w = tw;
	        h = th;
	        return this;
	    }
	    
	    /**
	     * set rectangle components using axis aware mode
	     * @param axis
	     * @param axisPos
	     * @param otherAxisPos
	     * @param axisSize
	     * @param otherAxisSize
	     * @return
	     */
	    public RectFloat Set(Axis2D axis, float axisPos, float otherAxisPos, float axisSize, float otherAxisSize)
	    {
		    return axis switch
		    {
			    Axis2D.X => Set(axisPos, otherAxisPos, axisSize, otherAxisSize),
			    Axis2D.Y => Set(otherAxisPos, axisPos, otherAxisSize, axisSize),
			    _ => this
		    };
	    }

	    /**
	     * copy provided rectangle into this
	     * @return 
	     */
	    public RectFloat Set(RectFloat r) {
	        x = r.x;
	        y = r.y;
	        w = r.w;
	        h = r.h;
	        return this;
	    }
	    
	    /**
	     * min x-coordinate retrieval
	     */
	    public float GetMinX() {
	        return x;
	    }

	    /**
	     * min y-coordinate retrieval
	     */
	    public float GetMinY() {
	        return y;
	    }
	    
	    /**
	     * max x-coordinate retrieval
	     */
	    public float GetMaxX() {
	        return x + w;
	    }

	    /**
	     * max y-coordinate retrieval
	     */
	    public float GetMaxY() {
	        return y + h;
	    }

	    /**
	     * check if this rect intersects with specified (intersection area > 0)
	     */
	    public bool Intersects(RectFloat r) {
	        var x0 = System.Math.Max(x, r.x);
	        var x1 = System.Math.Min(x + w, r.x + r.w);
	        var dx = x1 - x0;
	        if(dx <= 0) {
	            return false;
	        }
	        var y0 = System.Math.Max(y, r.y);
	        var y1 = System.Math.Min(y + h, r.y + r.h);
	        var dy = y1 - y0;
	        return !(dy <= 0);
	    }

	    /**
	     * check if rect is empty, i.e. have no area
	     */
	    public bool IsEmpty() {
	        return w <= 0f || h <= 0f;
	    }

	    /**
	     * reset all components to 0
	     */
	    public RectFloat Reset() {
	        x = y = w = h = 0;
	        return this;
	    }

	    /**
	     * move rectangle by dx, dy
	     */
	    public RectFloat MoveBy(float dx, float dy) {
	        x += dx;
	        y += dy;
	        return this;
	    }

	    /**
	     * x-coordinate of rectangle center retrieval
	     */
	    public float GetCenterX() {
	        return w == 0 ? x : x + w / 2f;
	    }
	    
	    /**
	     * y-coordinate of rectangle center retrieval
	     */
	    public float GetCenterY() {
	        return h == 0 ? y : y + h / 2f;
	    }

	    /**
	     * check whether this rectangle contains specified
	     * @param r
	     * @return
	     */
	    public bool Contains(RectFloat r) {
	        return Contains(r.x, r.y) && Contains(r.x + r.w, r.y + r.h);
	    }

	    /**
	     * move rect to specified position
	     */
	    public RectFloat MoveTo(float x2, float y2) {
	        x = x2;
	        y = y2;
	        return this;
	    }

	    /**
	     * creates copy of this rect
	     */
	    public RectFloat Copy() {
	        return new RectFloat().Set(this);
	    }

	    /**
	     * expand bounds by given amount in each direction
	     * @param d
	     * @return
	     */
	    public RectFloat Expand(float d) {
	        x -= d;
	        y -= d;
	        w += d * 2;
	        h += d * 2;
	        return this;
	    }
	    
	    /**
	     * expand bounds by given amount in each direction
	     */
	    public RectFloat Expand(float dx, float dy) {
	        x -= dx;
	        y -= dy;
	        w += dx * 2;
	        h += dy * 2;
	        return this;
	    }
	    
	    /**
	     * expand bounds by adding insets
	     */
	    /*public void expand(InsetsFloat insets) {
			x -= insets.left;
			y -= insets.bottom;
			w += insets.right + insets.left;
			h += insets.top + insets.bottom;
		}*/
	    
	    /**
	     * reduce bounds by subtracting insets
	     */
	    /*public void reduce(InsetsFloat insets) {
			x += insets.left;
			y += insets.bottom;
			w -= insets.right + insets.left;
			h -= insets.top + insets.bottom;
		}*/
	    
	    /**
	     * expand bounds by adding insets
	     */
	    public void Expand(float top, float left, float bottom, float right) {
			x -= left;
			y -= bottom;
			w += right + left;
			h += top + bottom;
		}

	    /**
	     * move rect so its center is at (cx, cy)
	     * @return 
	     */
	    public RectFloat MoveCenterTo(float cx, float cy) {
	        x = cx - w / 2f;
	        y = cy - h / 2f;
	        return this;
	    }
	    
	    public RectFloat MoveCenterTo(Vector2DFloat pos) {
    		return MoveCenterTo(pos.X, pos.Y);
	    }
	    
	    public RectFloat MoveCenterTo(RectFloat rc) {
    		return MoveCenterTo(rc.GetCenterX(), rc.GetCenterY());
	    }

	    /**
	     * set rectangle bounds from diagonal
	     * @return this
	     */
	    public RectFloat SetDiagonal(float x0, float y0, float x1, float y1) {
	        x = System.Math.Min(x0,  x1);
	        y = System.Math.Min(y0,  y1);
	        w = System.Math.Abs(x0 - x1);
	        h = System.Math.Abs(y0 - y1);
	        return this;
	    }
	    

		public RectFloat SetDiagonal(Vector2DFloat pt0, Vector2DFloat pt1) {
			return SetDiagonal(pt0.X, pt0.Y, pt1.X, pt1.Y);
		}
		
	    /**
	     * Support
	     * rect transformation support class
	     * 
	     * @author timur
	     */
	    /*public static class Support {
	        AffineTransform tx = AffineTransform.newInstance();
	        PointFloat ptSrc = new PointFloat();
	        PointFloat ptDst = new PointFloat();
	    }*/
	    
	    /**
	     * rotate this rectangle around anchor point and get bounding box of rotated shape
	     * @param theta an angle to rotate (radians, counter clockwise)
	     * @param anchorx rotation point x-component
	     * @param anchory rotation point y-component
	     * @param support for temporary use (may be null)
	     * @return this
	     */
	    /*
	    public RectFloat rotate(float theta, float anchorx, float anchory, Support support) {
	        if(support == null) {
	            support = new Support();
	        }
	        support.tx.SetToRotation(theta, anchorx, anchory);
	        float x0 = x;
	        float y0 = y;
	        float x1 = getMaxX();
	        float y1 = getMaxY();
	        support.tx.transform(support.ptSrc.Set(x0, y0), support.ptDst);
	        set(support.ptDst.x, support.ptDst.y, 0, 0);
	        add(support.tx.transform(support.ptSrc.Set(x1, y0), support.ptDst));
	        add(support.tx.transform(support.ptSrc.Set(x1, y1), support.ptDst));
	        add(support.tx.transform(support.ptSrc.Set(x0, y1), support.ptDst));
	        return this;
	    }*/
	    

	    /**
	     * checks whether x <= px <= x + w
	     */
	    public bool ContainsX(float px) {
	        return x <= px && px <= GetMaxX();
	    }
	    
	    /**
	     * checks whether y <= py <= y + h
	     */
	    public bool ContainsY(float py) {
	        return y <= py && py <= GetMaxY();
	    }

	    /**
	     * set diagonal in axis aware mode
	     * @param axis
	     * @param minAxis
	     * @param minOtherAxis
	     * @param maxAxis
	     * @param maxOtherAxis
	     */
	    public void SetDiagonal(Axis2D axis, float minAxis, float minOtherAxis, float maxAxis, float maxOtherAxis) {
	        switch(axis) {
	        case Axis2D.X:
	            SetDiagonal(minAxis, minOtherAxis, maxAxis, maxOtherAxis);
	            break;
	        case Axis2D.Y:
	            SetDiagonal(minOtherAxis, minAxis, maxOtherAxis, maxAxis);
	            break;
	        }
	    }

	    /**
	     * multiply all rectangle components by f
	     * @param f
	     */
	    public RectFloat Mul(float f) {
			x *= f;
			y *= f;
			w *= f;
			h *= f;
			return this;
		}
	    
	    public RectFloat Mul(float f, float scalingCenterPercentX, float scalingCenterPercentY) {
	        x = x + w * scalingCenterPercentX * (1 - f);
	        y = y + h * scalingCenterPercentY * (1 - f);
	        w *= f;
	        h *= f;
	        return this;
	    }
	    
	    /**
	     * scale using origin point
	     */
	    public RectFloat Scale(float scale, float originX, float originY) {
    		float f = 1f - scale;
    		x += (originX - x) * f;
    		y += (originY - y) * f;
    		w *= scale;
    		h *= scale;
    		return this;
	    }
	    
	    public RectFloat Scale(float scaleX, float scaleY, float originX, float originY) {
    		float fx = 1f - scaleX;
    		x += (originX - x) * fx;
    		float fy = 1f - scaleY;
    		y += (originY - y) * fy;
    		w *= scaleX;
    		h *= scaleY;
    		return this;
	    }
		
		/**
		 * scale rectangle so its center remains at same position and linear dimensions multiplied by specified factor
		 */
		public RectFloat ScaleFromCenter(float s) {
			return ScaleFromCenter(s, s);
		}
		
		/**
		 * scale rectangle so its center remains at same position and linear dimensions multiplied by specified factor
		 */
		public RectFloat ScaleFromCenter(float sx, float sy) {
			x -= (w * sx - w) / 2f;
			y -= (h * sy - h) / 2f;
			w *= sx;
			h *= sy;
			return this;
		}

		/**
		 * normalize rectangle so width/height is not negative
		 */
		public RectFloat Normalize() {
			if(w < 0) {
				x += w;
				w = -w;
			}
			if(h < 0) {
				y += h;
				h = -h;
			}
			return this;
		}

		/**
		 * multiply horizontally (x, w)
		 */
		public RectFloat MulHorz(float f) {
			x *= f;
			w *= f;
			return this;
		}
		
		/**
		 * multiply vertically (x, w)
		 */
		public RectFloat MulVert(float f) {
			y *= f;
			h *= f;
			return this;
		}

		/**
		 * translate rectangle so its center is at specified point
		 */
		public RectFloat SetCenter(Vector2DFloat c) {
			x = c.X - w / 2f; 
			y = c.Y - h / 2f;
			return this;
		}
		
		/**
		 * translate rectangle so its center is at specified point
		 */
		public RectFloat SetCenter(float cx, float cy) {
			x = cx - w / 2f; 
			y = cy - h / 2f;
			return this;
		}
		
		/**
		 * translate/resize rectangle so its center is at specified point
		 */
		public void SetCenter(float cx, float cy, float width, float height) {
			SetSize(width, height); 
			SetCenter(cx, cy);
		}

		/**
		 * set rectangle size
		 */
		public RectFloat SetSize(float width, float height) {
			w = width;
			h = height;
			return this;
		}

		/**
		 * check whether this rectangle contains specified point
		 */
		public bool Contains(Vector2DFloat pt) {
			return Contains(pt.X, pt.Y);
		}
		
		/**
		 * flip axis, i.e. change axis direction
		 */
		public RectFloat FlipAxis(bool flipX, bool flipY) {
			if(flipX) {
				x = -x - w;
			}
			if(flipY) {
				y = -y - h;
			}
			return this;
		}

		/**
		 * apply transform on this rectangle,
		 * result rectangle is minimal rectangle that contains all the points of transformed rectangle
		 * @param ptDst a point to reuse, might be null
		 * @param ptSrc a point to reuse, might be null
		 */
		/*@Deprecated
		public void transform(AffineTransform tx, PointFloat ptDst, PointFloat ptSrc) {
			float xmin = float.NaN;
			float xmax = float.NaN;
			float ymin = float.NaN;
			float ymax = float.NaN;
			if(ptSrc == null) {
				ptSrc = new PointFloat();
			}
			ptSrc.Set(x, y);
			ptDst = tx.transform(ptSrc, ptDst);
			xmin = ptDst.x;
			ymin = ptDst.y;
			xmax = ptDst.x;
			ymax = ptDst.y;

			ptSrc.Set(x + w, y);
			ptDst = tx.transform(ptSrc, ptDst);
			xmin = System.Math.Min(xmin, ptDst.x);
			ymin = System.Math.Min(ymin, ptDst.y);
			xmax = System.Math.Max(xmax, ptDst.x);
			ymax = System.Math.Max(ymax, ptDst.y);
			
			ptSrc.Set(x + w, y + h);
			ptDst = tx.transform(ptSrc, ptDst);
			xmin = System.Math.Min(xmin, ptDst.x);
			ymin = System.Math.Min(ymin, ptDst.y);
			xmax = System.Math.Max(xmax, ptDst.x);
			ymax = System.Math.Max(ymax, ptDst.y);
			
			ptSrc.Set(x, y + h);
			ptDst = tx.transform(ptSrc, ptDst);
			xmin = System.Math.Min(xmin, ptDst.x);
			ymin = System.Math.Min(ymin, ptDst.y);
			xmax = System.Math.Max(xmax, ptDst.x);
			ymax = System.Math.Max(ymax, ptDst.y);
			
			setDiagonal(xmin, ymin, xmax, ymax);
		}
		
		@Deprecated
		public void transform(AffineTransform tx) {
			transform(tx, null, null);
		}*/

		/**
		 * set position
		 * @return 
		 */
		public RectFloat SetPos(float tx, float ty) {
			x = tx;
			y = ty;
			return this;
		}

		/**
		 * make this rectangle to be inside target, adjusting pos/size if necessary
		 * @return 
		 */
		public RectFloat Fit(RectFloat target) {
			if(w > target.w) {
				w = target.w;
			}
			if(h > target.h) {
				h = target.h;
			}
			if(x < target.x) {
				x = target.x;
			}
			if(y < target.y) {
				y = target.y;
			}
			if(GetMaxX() > target.GetMaxX()) {
				x = target.GetMaxX() - w;
			}
			if(GetMaxY() > target.GetMaxY()) {
				y = target.GetMaxY() - h;
			}
			return this;
		}

		/**
		 * retrieve point on the edge or in the center of rectangle that matches direction
		 * @param target point to reuse, may be null
		 */
		public Vector2DFloat GetPoint(Dir dir, Vector2DFloat target) {
			if(target == null) {
				target = new Vector2DFloat();
			}
			target.Set(GetX(dir), GetY(dir));
			return target;
		}
		
		public float GetX(Dir dir) {
			return GetCenterX() + w * dir.X() / 2f;
		}
		
		public float GetY(Dir dir) {
			return GetCenterY() + h * dir.Y() / 2f;
		}
		
		/**
		 * retrieve corner secondary direction (NW, NE, SW, SE) of rect corner
		 * @param px
		 * @param py
		 * @return dir or null if point is not corner
		 */
		public Dir GetCorner(float px, float py) {
			if(px == x) {
				if(py == y) {
					return Dir.SW;
				} else if(py == y + h) {
					return Dir.NW;
				}
			} else if(px == x + w) {
				if(py == y) {
					return Dir.SE;
				} else if(py == y + h) {
					return Dir.NE;
				}
			}
			return default;
		}
		
		/**
		 * retrieve quadrant direction of point relative to this rectangle
		 * @param correction if point is on quadrants border, 
		 * then move by 0 in specified directory to determine proper quadrant
		 */
		public Dir GetQuadrant(float tx, float ty, Dir correction) {
			int dx = 0, dy = 0;
			if(tx < x) {
				dx = DirEx.NEGATIVE;
			} else if(tx > GetMaxX()) {
				dx = DirEx.POSITIVE;
			}
			if(ty < y) {
				dy = DirEx.NEGATIVE;
			} else if(ty > GetMaxY()) {
				dy = DirEx.POSITIVE;
			}
			if(correction != null) {
				if((tx == x && correction.X() < 0) || 
				   (correction.X() > 0 && tx == GetMaxX())) {
					dx = correction.X();
				}
				if((ty == y && correction.Y() < 0) || 
				   (correction.Y() > 0 && ty == GetMaxY())) {
					dy = correction.Y();
				}
			}
			var result = DirEx.valueOf(dx, dy);
			return result;
		}

		/**
		 * retrieve transform that transforms this rect to target
		 * @param target target rectangle
		 * @param result a transform to return, optional
		 * @return transform required to transform this rectangle into target
		 */
		/*public AffineTransform getTransform(RectFloat target, AffineTransform result) {
			if(result == null) {
				result = AffineTransform.newInstance();
			}
			result.SetToIdentity();
			float sx = target.w / w;
			float sy = target.h / h;
			result.translate((target.x - x) * sx, (target.y - y) * sy);
			result.scale(sx, sy);
			return result;
		}*/

		/**
		 * set aspect ratio (w/h) for this rectangle, this will shrink rectangle
		 */
		public void SetAspectRatio(float r) {
			float currentRatio = w / h;
			if(currentRatio > r) {
				w = h  * r;
			} else {
				h = w / r;
			}
		}
		
		/**
		 * align this rect to edge of anchor rect so this rect
		 */
		public RectFloat AlignOutside(RectFloat anchor, Dir dir) {
			if(dir.IsRight()) {
				x = anchor.GetMaxX();
			} else if(dir.IsLeft()) {
				x = anchor.GetMinX() - w;
			}
			if(dir.IsUp()) {
				y = anchor.GetMaxY();
			} else if(dir.IsDown()) {
				y = anchor.GetMinY() - h;
			}
			return this;
		}

		public RectFloat SetPos(Vector2DFloat pt) {
			return SetPos(pt.X, pt.Y);
		}

		/**
		 * return parameter or new instance if null passed
		 */
		public static RectFloat Nvl(RectFloat bounds) {
			return bounds ?? new RectFloat();
		}
		
		public override bool Equals(object obj)
		{
			if (obj is RectFloat o2)
			{
				return Equals(obj);
			}

			return false;
		}
		
		protected bool Equals(RectFloat other)
		{
			return x.Equals(other.x) && y.Equals(other.y) && w.Equals(other.w) && h.Equals(other.h);
		}

		/**
		 * swap width/height
		 */
		public RectFloat SwapSize() {
			(w, h) = (h, w);
			return this;
		}

		/**
		 * set width/height ratio
		 * @param f a ratio to set (width/height)
		 * @param keepWidth if true, width remains unchanged, otherwise height
		 */
		public RectFloat SetRatio(float f, bool keepWidth) {
			if(keepWidth) {
				h = w / f;
			} else {
				w = h * f;
			}
			return this;
		}
		
		/**
		 * ratio retrieval (width/height)
		 */
		public float GetRatio() {
			return w / h;
		}

		/**
		 * set all components to nan
		 */
		public RectFloat SetNaN() {
			x = y = w = h = float.NaN;
			return this;
		}

		/**
		 * check if any component is NaN
		 */
		public bool IsNaN() {
			return float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(w) || float.IsNaN(h);
		}

		public float GetDiagonal() {
			return (float) System.Math.Sqrt(w * w + h * h);
		}

		public RectFloat SetSize(Vector2DFloat size) {
			w = size.X;
			h = size.Y;
			return this;
		}

		public RectFloat MoveBy(Vector2DFloat pt) {
			return MoveBy(pt.X, pt.Y);
		}
		
		/**
		 * rotate by 90 degrees using center as anchor
		 */
		public RectFloat Rot90() {
			var cx = GetCenterX();
			var cy = GetCenterY();
			SwapSize();
			MoveCenterTo(cx, cy);
			return this;
		}

		/**
		 * set edge position to specified value
		 * @param dir a primary directory of edge
		 * @param pos a new position of edge to set
		 * @param keepOppositeEdge shows whether opposite edge should keep its position
		 */
		public void SetEdgePos(Dir dir, float pos, bool keepOppositeEdge) {
			LangHelper.Validate(dir.IsPrimary());
			var current = GetEdgePos(dir);
			var d = pos - current;
			if(keepOppositeEdge) {
				var invDir = dir.Invert();
				var invEdgePos = GetEdgePos(invDir);
				SetAxisBounds(pos, invEdgePos, dir.IsHorz());
			} else {
				var horz = dir.IsHorz();
				var dx = horz ? d : 0;
				var dy = horz ? 0 : d;
				MoveBy(dx, dy);
			}
		}
		
		/**
		 * move this rect by specified amount in given direction
		 */
		public void MoveBy(float d, Dir dir) {
			var dx = dir.X() * d;
			var dy = dir.Y() * d;
			MoveBy(dx, dy);
		}

		/**
		 * retrieve edge position of specified primary dir
		 * @param dir
		 * @return
		 */
		public float GetEdgePos(Dir dir)
		{
			return dir switch
			{
				Dir.E => GetMaxX(),
				Dir.N => GetMaxY(),
				Dir.S => GetMinY(),
				Dir.W => GetMinX(),
				_ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
			};
		}
		
		/**
		 * set edge positions to specified values
		 * @param v0 first edge position
		 * @param v1 second edge position
		 * @param horz true if use horizontal (x) axis
		 */
		public void SetAxisBounds(float v0, float v1, bool horz) {
			if(horz) {
				x = System.Math.Min(v0, v1);
				w = System.Math.Abs(v0 - v1);
			} else {
				y = System.Math.Min(v0, v1);
				h = System.Math.Abs(v0 - v1);
			}
		}
		
		/**
		 * retrieve size for specified axis
		 * @param horz
		 * @return
		 */
		public float GetSize(bool horz) {
			return horz ? w : h;
		}

		public bool HitTest(float tx, float ty) {
			return Contains(tx, ty);
		}

		public void FindClosestEdgePos(float x, float y, Vector2DFloat target)
		{
			throw new NotImplementedException();
		}

		/**
		 * retrieve edge as segment
		 */
		/*public SegmentFloat getSegment(Dir dir, SegmentFloat target) {
			LangHelper.validate(dir.isPrimary());
			if(target == null) {
				target = new SegmentFloat();
			}
			float maxX = getMaxX();
			float maxY = getMaxY();
			switch(dir) {
			case E:
				target.Set(maxX, y, maxX, maxY);
				break;
			case N:
				target.Set(x, maxY, maxX, maxY);
				break;
			case S:
				target.Set(x, y, maxX, y);
				break;
			case W:
				target.Set(x, y, x, maxY);
				break;
			}
			return target;
		}

		@Override
		public void findClosestEdgePos(float tx, float ty, PointFloat target) {
			if(_segment == null) {
				_segment = new SegmentFloat();
			}
			synchronized(_segment) {
				float bestD2 = Float.MAX_VALUE;
				Dir bestDir = null;
				float bestX = 0, bestY = 0;
				for(Dir dir : Dir.PRIMARY) {
					getSegment(dir, _segment);
					_segment.getClosestPointOnSegment(tx, ty, target);
					float d2 = target.distance2(tx, ty);
					if(bestDir == null || d2 < bestD2) {
						bestD2 = d2;
						bestX = target.x;
						bestY = target.y;
						bestDir = dir;
					}
				}
				target.Set(bestX, bestY);
			}
		}
		
		@Override
		public void randomPointInside(Randomizer rnd, PointFloat target) {
			target.x = x + rnd.randomFloat(w);
			target.y = y + rnd.randomFloat(h);
		}
		*/

		public void GetBounds(RectFloat target) {
			target.Set(this);
		}

		public void SetSize(Dir dir, float tw, float th) {
			var ox = GetX(dir);
			var oy = GetY(dir);
			var sx = tw / w;
			var sy = th / h;
			Scale(sx, sy, ox, oy);
		}
		
		/**
		 * align this rect horizontally using bounds and scale factor
		 */
		public RectFloat AlignHorz(RectFloat bounds, float scale) {
			var halfSize = w / 2f;
			var min = bounds.GetMinX() + halfSize;
			var max = bounds.GetMaxX() - halfSize;
			var c = MathHelper.Lerp(min, max, scale);
			x = c - halfSize;
			return this;
		}
		
		/**
		 * align this rect vertically using bounds and scale factor
		 */
		public RectFloat AlignVert(RectFloat bounds, float scale) {
			var halfSize = h / 2f;
			var min = bounds.GetMinY() + halfSize;
			var max = bounds.GetMaxY() - halfSize;
			var c = MathHelper.Lerp(min, max, scale);
			y = c - halfSize;
			return this;
		}
	}
}
