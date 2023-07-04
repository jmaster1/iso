using Common.Lang;

namespace Common.Util.Math.Path.AStar
{
    public class Node<TC> : IClearable where TC : class
    {
        /**
		 * cell reference
		 */
        internal TC Cell;
	
        /**
		 * previous node
		 */
        internal Node<TC> CameFrom;
	
        /**
		 * g-score
		 */
        internal int G;
	
        /**
		 * h-score
		 */
        internal int H;
	
        /**
		 * f-score
		 */
        public int F;
        
        public void Clear()
        {
	        Cell = null;
	        CameFrom = null;
	        G = 0;
	        H = 0;
	        F = 0;
        }
    }
}