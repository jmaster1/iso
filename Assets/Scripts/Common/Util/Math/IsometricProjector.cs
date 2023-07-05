namespace Common.Util.Math
{
    /**
     * IsometricProjector
     * projects orthogonal model coordinates to isometric view.
     * This also implements Comparator<AbstractGdxRenderer> - 
     * this allows to sort views for proper rendering sequence of renderers 
     * 
     * @author timur
     */
    public class IsometricProjector
    {
        /**
	 * isometric tile half width 
	 */
        public float halfTileWidth = 0.5f;
	
        /**
	 * isometric tile half height 
	 */
        public float halfTileHeight = 0.5f;

        public float m2vx(float x, float y) {
            return (x * halfTileWidth) - (y * halfTileWidth);
        }

        public float m2vy(float x, float y) {
            return (y * halfTileHeight) + (x * halfTileHeight);
        }

        public float v2mx(float x, float y) {
            return ((x / halfTileWidth) + (y / halfTileHeight)) / 2f;
        }

        public float v2my(float x, float y) {
            return ((y / halfTileHeight) - (x / halfTileWidth)) / 2f;
        }
    }
}