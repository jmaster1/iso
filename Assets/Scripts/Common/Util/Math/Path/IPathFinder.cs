using System.Collections.Generic;

namespace Common.Util.Math.Path
{
    public interface IPathFinder<TC>
    {
        /**
	     * find path for start > goal in a graph
	     * @return list (always returned same instance) or null, if no path
	     */
        List<TC> FindPath(IGraph<TC> graph, TC start, TC goal);
    }
}
