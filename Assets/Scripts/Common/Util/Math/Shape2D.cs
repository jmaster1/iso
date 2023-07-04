using Math;

namespace Common.Util.Math
{
    /**
     * Shape2D
     * query api for 2d closed shape
     * 
     * @author timur
     *
     */
    public interface Shape2D {
    
        /**
         * check if specified point is inside this shape
         * @param x test point x
         * @param y test point y
         * @return
         */
        bool hitTest(float x, float y);
        
        /**
         * find point closest to specified on shape contour
         * @param x test point x
         * @param y test point y
         * @param target will receive function result, must be not null
         */
        void findClosestEdgePos(float x, float y, Vector2DFloat target);
        
        /**
         * retrieve random point inside shape
         * @param target will receive function result, must be not null
         */
        //void randomPointInside(Randomizer rnd, PointFloat target);
        
        /**
         * retrieve axis aligned bounding box of this shape
         * @param target
         */
        void getBounds(RectFloat target);
    }
}
