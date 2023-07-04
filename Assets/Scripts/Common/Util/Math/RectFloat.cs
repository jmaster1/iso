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
		
		public static readonly int PT_LEFT_BOTTOM = 0;
		public static readonly int PT_RIGHT_BOTTOM = 1;
		public static readonly int PT_RIGHT_TOP = 2;
		public static readonly int PT_LEFT_TOP= 3;
		
		/**
		 * initialize array of 4 points that match rectangle corners
		 * @return array of points, use PT_?_? constants to access points
		 */
		public static Vector2DFloat[] getRectPoints(float x, float y, float w, float h, Vector2DFloat[] target) {
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
    		setDiagonal(pt0.X, pt0.Y, pt1.X, pt1.Y);
		}

	    /**
	     * copy constructor
	     */
		public RectFloat(RectFloat rc) {
			set(rc);
		}

	    /**
	     * check if rectangle contains specified point
	     * @return true if point is inside or on the edge
	     */
	    public bool contains(float px, float py) {
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
	    public bool add(float x2, float y2) {
	        bool result = false;
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
	    
	    public bool add(Vector2DFloat pt) {
	        return add(pt.X, pt.Y);
	    }

	    /**
	     * expand rectangle (if necessary) so specified rectangle contained by this
	     * 
	     * @param bounds
	     */
	    public bool add(RectFloat bounds) {
	        return add(bounds.x, bounds.y) |
	                add(bounds.x + bounds.w, bounds.y + bounds.h);
	    }
	    
	    /**
	     * expand rectangle (if necessary) so this contains specified circle
	     * @param x x-component of circle center
	     * @param y y-component of circle center
	     * @param r circle radius
	     */
	    public bool add(float x, float y, float r) {
	        return add(x - r, y - r) | add(x + r, y + r);
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
	    public RectFloat set(float x, float y, float w, float h) {
	        this.x = x;
	        this.y = y;
	        this.w = w;
	        this.h = h;
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
	    public RectFloat set(Axis2D axis, float axisPos, float otherAxisPos, float axisSize, float otherAxisSize)
	    {
		    return axis switch
		    {
			    Axis2D.X => set(axisPos, otherAxisPos, axisSize, otherAxisSize),
			    Axis2D.Y => set(otherAxisPos, axisPos, otherAxisSize, axisSize),
			    _ => this
		    };
	    }

	    /**
	     * copy provided rectangle into this
	     * @return 
	     */
	    public RectFloat set(RectFloat r) {
	        x = r.x;
	        y = r.y;
	        w = r.w;
	        h = r.h;
	        return this;
	    }
	    
	    /**
	     * min x-coordinate retrieval
	     */
	    public float getMinX() {
	        return x;
	    }

	    /**
	     * min y-coordinate retrieval
	     */
	    public float getMinY() {
	        return y;
	    }
	    
	    /**
	     * max x-coordinate retrieval
	     */
	    public float getMaxX() {
	        return x + w;
	    }

	    /**
	     * max y-coordinate retrieval
	     */
	    public float getMaxY() {
	        return y + h;
	    }

	    /**
	     * check if this rect intersects with specified (intersection area > 0)
	     */
	    public bool intersects(RectFloat r) {
	        float x0 = System.Math.Max(x, r.x);
	        float x1 = System.Math.Min(x + w, r.x + r.w);
	        float dx = x1 - x0;
	        if(dx <= 0) {
	            return false;
	        }
	        float y0 = System.Math.Max(y, r.y);
	        float y1 = System.Math.Min(y + h, r.y + r.h);
	        float dy = y1 - y0;
	        if(dy <= 0) {
	            return false;
	        }
	        return true;
	    }

	    /**
	     * check if rect is empty, i.e. have no area
	     */
	    public bool isEmpty() {
	        return w <= 0f || h <= 0f;
	    }

	    /**
	     * reset all components to 0
	     */
	    public RectFloat reset() {
	        x = y = w = h = 0;
	        return this;
	    }

	    /**
	     * move rectangle by dx, dy
	     */
	    public RectFloat moveBy(float dx, float dy) {
	        x += dx;
	        y += dy;
	        return this;
	    }

	    /**
	     * x-coordinate of rectangle center retrieval
	     */
	    public float getCenterX() {
	        return w == 0 ? x : x + w / 2f;
	    }
	    
	    /**
	     * y-coordinate of rectangle center retrieval
	     */
	    public float getCenterY() {
	        return h == 0 ? y : y + h / 2f;
	    }

	    /**
	     * check whether this rectangle contains specified
	     * @param r
	     * @return
	     */
	    public bool contains(RectFloat r) {
	        return contains(r.x, r.y) && contains(r.x + r.w, r.y + r.h);
	    }

	    /**
	     * move rect to specified position
	     */
	    public RectFloat moveTo(float x2, float y2) {
	        x = x2;
	        y = y2;
	        return this;
	    }

	    /**
	     * creates copy of this rect
	     */
	    public RectFloat copy() {
	        return new RectFloat().Set(this);
	    }

	    /**
	     * expand bounds by given amount in each direction
	     * @param d
	     * @return
	     */
	    public RectFloat expand(float d) {
	        x -= d;
	        y -= d;
	        w += d * 2;
	        h += d * 2;
	        return this;
	    }
	    
	    /**
	     * expand bounds by given amount in each direction
	     */
	    public RectFloat expand(float dx, float dy) {
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
	    public void expand(float top, float left, float bottom, float right) {
			x -= left;
			y -= bottom;
			w += right + left;
			h += top + bottom;
		}

	    /**
	     * move rect so its center is at (cx, cy)
	     * @return 
	     */
	    public RectFloat moveCenterTo(float cx, float cy) {
	        x = cx - w / 2f;
	        y = cy - h / 2f;
	        return this;
	    }
	    
	    public RectFloat moveCenterTo(Vector2DFloat pos) {
    		return moveCenterTo(pos.X, pos.Y);
	    }
	    
	    public RectFloat moveCenterTo(RectFloat rc) {
    		return moveCenterTo(rc.getCenterX(), rc.getCenterY());
	    }

	    /**
	     * set rectangle bounds from diagonal
	     * @return this
	     */
	    public RectFloat setDiagonal(float x0, float y0, float x1, float y1) {
	        x = System.Math.Min(x0,  x1);
	        y = System.Math.Min(y0,  y1);
	        w = System.Math.Abs(x0 - x1);
	        h = System.Math.Abs(y0 - y1);
	        return this;
	    }
	    

		public RectFloat setDiagonal(Vector2DFloat pt0, Vector2DFloat pt1) {
			return setDiagonal(pt0.X, pt0.Y, pt1.X, pt1.Y);
		}

	    public RectFloat setX(float x) {
	        this.x = x;
	        return this;
	    }

	    public RectFloat setY(float y) {
	        this.y = y;
	        return this;
	    }

	    public RectFloat setW(float w) {
	        this.w = w;
	        return this;
	    }

	    public RectFloat setH(float h) {
	        this.h = h;
	        return this;
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
	    public bool containsX(float px) {
	        return x <= px && px <= getMaxX();
	    }
	    
	    /**
	     * checks whether y <= py <= y + h
	     */
	    public bool containsY(float py) {
	        return y <= py && py <= getMaxY();
	    }

	    /**
	     * set diagonal in axis aware mode
	     * @param axis
	     * @param minAxis
	     * @param minOtherAxis
	     * @param maxAxis
	     * @param maxOtherAxis
	     */
	    public void setDiagonal(Axis2D axis, float minAxis, float minOtherAxis, float maxAxis, float maxOtherAxis) {
	        switch(axis) {
	        case Axis2D.X:
	            setDiagonal(minAxis, minOtherAxis, maxAxis, maxOtherAxis);
	            break;
	        case Axis2D.Y:
	            setDiagonal(minOtherAxis, minAxis, maxOtherAxis, maxAxis);
	            break;
	        }
	    }

	    /**
	     * multiply all rectangle components by f
	     * @param f
	     */
	    public RectFloat mul(float f) {
			x *= f;
			y *= f;
			w *= f;
			h *= f;
			return this;
		}
	    
	    public RectFloat mul(float f, float scalingCenterPercentX, float scalingCenterPercentY) {
	        x = x + w * scalingCenterPercentX * (1 - f);
	        y = y + h * scalingCenterPercentY * (1 - f);
	        w *= f;
	        h *= f;
	        return this;
	    }
	    
	    /**
	     * scale using origin point
	     */
	    public RectFloat scale(float scale, float originX, float originY) {
    		float f = 1f - scale;
    		x += (originX - x) * f;
    		y += (originY - y) * f;
    		w *= scale;
    		h *= scale;
    		return this;
	    }
	    
	    public RectFloat scale(float scaleX, float scaleY, float originX, float originY) {
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
		public RectFloat scaleFromCenter(float s) {
			return scaleFromCenter(s, s);
		}
		
		/**
		 * scale rectangle so its center remains at same position and linear dimensions multiplied by specified factor
		 */
		public RectFloat scaleFromCenter(float sx, float sy) {
			x -= (w * sx - w) / 2f;
			y -= (h * sy - h) / 2f;
			w *= sx;
			h *= sy;
			return this;
		}

		/**
		 * normalize rectangle so width/height is not negative
		 */
		public RectFloat normalize() {
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
		public RectFloat mulHorz(float f) {
			x *= f;
			w *= f;
			return this;
		}
		
		/**
		 * multiply vertically (x, w)
		 */
		public RectFloat mulVert(float f) {
			y *= f;
			h *= f;
			return this;
		}

		/**
		 * translate rectangle so its center is at specified point
		 */
		public RectFloat setCenter(Vector2DFloat c) {
			x = c.X - w / 2f; 
			y = c.Y - h / 2f;
			return this;
		}
		
		/**
		 * translate rectangle so its center is at specified point
		 */
		public RectFloat setCenter(float cx, float cy) {
			x = cx - w / 2f; 
			y = cy - h / 2f;
			return this;
		}
		
		/**
		 * translate/resize rectangle so its center is at specified point
		 */
		public void setCenter(float cx, float cy, float width, float height) {
			setSize(width, height); 
			setCenter(cx, cy);
		}

		/**
		 * set rectangle size
		 */
		public RectFloat setSize(float width, float height) {
			w = width;
			h = height;
			return this;
		}

		/**
		 * check whether this rectangle contains specified point
		 */
		public bool contains(Vector2DFloat pt) {
			return contains(pt.X, pt.Y);
		}
		
		/**
		 * flip axis, i.e. change axis direction
		 */
		public RectFloat flipAxis(bool flipX, bool flipY) {
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
		public RectFloat setPos(float x, float y) {
			this.x = x;
			this.y = y;
			return this;
		}

		/**
		 * make this rectangle to be inside target, adjusting pos/size if necessary
		 * @return 
		 */
		public RectFloat fit(RectFloat target) {
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
			if(getMaxX() > target.getMaxX()) {
				x = target.getMaxX() - w;
			}
			if(getMaxY() > target.getMaxY()) {
				y = target.getMaxY() - h;
			}
			return this;
		}

		/**
		 * retrieve point on the edge or in the center of rectangle that matches direction
		 * @param target point to reuse, may be null
		 */
		public Vector2DFloat getPoint(Dir dir, Vector2DFloat target) {
			if(target == null) {
				target = new Vector2DFloat();
			}
			target.Set(getX(dir), getY(dir));
			return target;
		}
		
		public float getX(Dir dir) {
			return getCenterX() + w * dir.X() / 2f;
		}
		
		public float getY(Dir dir) {
			return getCenterY() + h * dir.Y() / 2f;
		}
		
		/**
		 * retrieve corner secondary direction (NW, NE, SW, SE) of rect corner
		 * @param px
		 * @param py
		 * @return dir or null if point is not corner
		 */
		public Dir getCorner(float px, float py) {
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
		public Dir getQuadrant(float x, float y, Dir correction) {
			Dir result;
			int dx = 0, dy = 0;
			if(x < this.x) {
				dx = DirEx.NEGATIVE;
			} else if(x > getMaxX()) {
				dx = DirEx.POSITIVE;
			}
			if(y < this.y) {
				dy = DirEx.NEGATIVE;
			} else if(y > getMaxY()) {
				dy = DirEx.POSITIVE;
			}
			if(correction != null) {
				if(((x == this.x && correction.X() < 0) || 
						(correction.X() > 0 && x == getMaxX()))) {
					dx = correction.X();
				}
				if(((y == this.y && correction.Y() < 0) || 
						(correction.Y() > 0 && y == getMaxY()))) {
					dy = correction.Y();
				}
			}
			result = DirEx.valueOf(dx, dy);
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
		public void setAspectRatio(float r) {
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
		public RectFloat alignOutside(RectFloat anchor, Dir dir) {
			if(dir.IsRight()) {
				x = anchor.getMaxX();
			} else if(dir.IsLeft()) {
				x = anchor.getMinX() - w;
			}
			if(dir.IsUp()) {
				y = anchor.getMaxY();
			} else if(dir.IsDown()) {
				y = anchor.getMinY() - h;
			}
			return this;
		}

		public RectFloat setPos(Vector2DFloat pt) {
			return setPos(pt.X, pt.Y);
		}

		/**
		 * return parameter or new instance if null passed
		 */
		public static RectFloat nvl(RectFloat bounds) {
			return bounds == null ? new RectFloat() : bounds;
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
		public RectFloat swapSize() {
			float tmp = w;
			w = h;
			h = tmp;
			return this;
		}

		/**
		 * set width/height ratio
		 * @param f a ratio to set (width/height)
		 * @param keepWidth if true, width remains unchanged, otherwise height
		 */
		public RectFloat setRatio(float f, bool keepWidth) {
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
		public float getRatio() {
			return w / h;
		}

		/**
		 * set all components to nan
		 */
		public RectFloat setNaN() {
			x = y = w = h = float.NaN;
			return this;
		}

		/**
		 * check if any component is NaN
		 */
		public bool isNaN() {
			return float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(w) || float.IsNaN(h);
		}

		public float getDiagonal() {
			return (float) System.Math.Sqrt(w * w + h * h);
		}

		public RectFloat setSize(Vector2DFloat size) {
			w = size.X;
			h = size.Y;
			return this;
		}

		public RectFloat moveBy(Vector2DFloat pt) {
			return moveBy(pt.X, pt.Y);
		}
		
		/**
		 * rotate by 90 degrees using center as anchor
		 */
		public RectFloat rot90() {
			var cx = getCenterX();
			var cy = getCenterY();
			swapSize();
			moveCenterTo(cx, cy);
			return this;
		}

		/**
		 * set edge position to specified value
		 * @param dir a primary directory of edge
		 * @param pos a new position of edge to set
		 * @param keepOppositeEdge shows whether opposite edge should keep its position
		 */
		public void setEdgePos(Dir dir, float pos, bool keepOppositeEdge) {
			LangHelper.Validate(dir.IsPrimary());
			var current = getEdgePos(dir);
			var d = pos - current;
			if(keepOppositeEdge) {
				var invDir = dir.Invert();
				var invEdgePos = getEdgePos(invDir);
				setAxisBounds(pos, invEdgePos, dir.IsHorz());
			} else {
				var horz = dir.IsHorz();
				var dx = horz ? d : 0;
				var dy = horz ? 0 : d;
				moveBy(dx, dy);
			}
		}
		
		/**
		 * move this rect by specified amount in given direction
		 */
		public void moveBy(float d, Dir dir) {
			var dx = dir.X() * d;
			var dy = dir.Y() * d;
			moveBy(dx, dy);
		}

		/**
		 * retrieve edge position of specified primary dir
		 * @param dir
		 * @return
		 */
		public float getEdgePos(Dir dir)
		{
			LangHelper.Validate(dir.IsPrimary());
			return dir switch
			{
				Dir.E => getMaxX(),
				Dir.N => getMaxY(),
				Dir.S => getMinY(),
				Dir.W => getMinX()
			};
		}
		
		/**
		 * set edge positions to specified values
		 * @param v0 first edge position
		 * @param v1 second edge position
		 * @param horz true if use horizontal (x) axis
		 */
		public void setAxisBounds(float v0, float v1, bool horz) {
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
		public float getSize(bool horz) {
			return horz ? w : h;
		}

		public bool hitTest(float tx, float ty) {
			return contains(tx, ty);
		}

		public void findClosestEdgePos(float x, float y, Vector2DFloat target)
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

		public void getBounds(RectFloat target) {
			target.Set(this);
		}

		private RectFloat Set(RectFloat other)
		{
			x = other.x;
			y = other.y;
			w = other.w;
			h = other.h;
			return this;
		}

		public void setSize(Dir dir, float w, float h) {
			var ox = getX(dir);
			var oy = getY(dir);
			var sx = w / this.w;
			var sy = h / this.h;
			scale(sx, sy, ox, oy);
		}
		
		/**
		 * align this rect horizontally using bounds and scale factor
		 */
		public RectFloat alignHorz(RectFloat bounds, float scale) {
			var halfSize = w / 2f;
			var min = bounds.getMinX() + halfSize;
			var max = bounds.getMaxX() - halfSize;
			var c = MathHelper.Lerp(min, max, scale);
			x = c - halfSize;
			return this;
		}
		
		/**
		 * align this rect vertically using bounds and scale factor
		 */
		public RectFloat alignVert(RectFloat bounds, float scale) {
			var halfSize = h / 2f;
			var min = bounds.getMinY() + halfSize;
			var max = bounds.getMaxY() - halfSize;
			var c = MathHelper.Lerp(min, max, scale);
			y = c - halfSize;
			return this;
		}
	}
}