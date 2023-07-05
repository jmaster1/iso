using System.Collections.Generic;
using Common.Api.Pool;

namespace Common.Util.Math.Path.AStar
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/A*_search_algorithm
    /// </summary>
    /// <typeparam name="TC"></typeparam>
    public class AStarPathFinder<TC> : IPathFinder<TC> where TC : class
    {
        private readonly Pool<Node<TC>> nodePool = new();

        /**
         * cost of moving by 1 cell
         */
        private readonly int cellMoveCost = 1;

        /**
         * search closed nodes
         */
        private readonly NodeList<TC> closed = new();

        /**
         * search open nodes
         */
        private readonly NodeList<TC> open = new();

        /**
         * grid
         */
        private IGraph<TC> grid;

        /**
         * start cell
         */
        private TC start;

        /**
         * target cell
         */
        private TC goal;

        /**
         * result
         */
        private readonly List<TC> path = new();

        public List<TC> FindPath(IGraph<TC> grid, TC start, TC goal) {
            this.grid = grid;
            this.start = start;
            this.goal = goal;
            var startNode = GetNode(start, null);
            open.Add(startNode);
            while (!open.IsEmpty()) {
                var node = open.BestNode();
                if (node.Cell == goal) {
                    ReconstructPath(node, path);
                    Cleanup();
                    return path;
                }
                open.Remove(node);
                closed.Add(node);
                for (int i = 0, n = grid.GetSiblingCount(node.Cell); i < n; i++) {
                    var nextCell = grid.GetSibling(node.Cell, i);
                    if (nextCell == null || closed.Contains(nextCell)) {
                        continue;
                    }
                    var tentativeGScore = node.G + Distance(node.Cell, nextCell);
                    Node<TC> nextNode = null;
                    if (open.Contains(nextCell)) {
                        nextNode = open.GetNode(nextCell);
                        if (tentativeGScore >= nextNode.G) continue;
                        nextNode.CameFrom = node;
                        nextNode.G = tentativeGScore;
                        nextNode.H = H(nextNode.Cell, goal);
                        open.SetF(nextNode, nextNode.G + nextNode.H);
                    } else {
                        nextNode = GetNode(nextCell, node);
                        nextNode.CameFrom = node;
                        nextNode.G = tentativeGScore;
                        nextNode.H = H(nextNode.Cell, goal);
                        nextNode.F = nextNode.G + nextNode.H;
                        open.Add(nextNode);
                    }
                }
            }
            Cleanup();
            return null;
        }

        /**
         * cleanup state, should be called after each findPath() call
         */
        private void Cleanup() {
            open.Reset(nodePool);
            closed.Reset(nodePool);
        }

        /**
         * acquire node
         */
        private Node<TC> GetNode(TC cell, Node<TC> cameFrom)
        {
            var node = nodePool.Get();
            Init(node, cell, cameFrom);
            return node;
        }

        private static void ReconstructPath(Node<TC> end, IList<TC> path) {
            path.Clear();
            while (end != null) {
                path.Add(end.Cell);
                end = end.CameFrom;
            }
            //
            // reverse
            var n = path.Count - 1;
            for (var i = n / 2; i >= 0; i--) {
                var temp = path[i];
                int ii;
                path[i] = path[ii = n - i];
                path[ii] = temp;
            }
        }

        /**
         * heuristic distance retrieval between cells
         */
        private int H(TC from, TC to) {
            return Distance(from, to);
        }

        /**
         * distance retrieval between cells
         */
        private int Distance(TC from, TC to) {
            return cellMoveCost * grid.Distance(from, to);
        }

        /**
         * initialize with cell and previous node
         * @param cell
         * @param cameFrom
         */
        private void Init(Node<TC> node, TC cell, Node<TC> cameFrom) {
            node.Cell = cell;
            node.CameFrom = cameFrom;
            node.G = cameFrom?.G + cellMoveCost ?? 0;
            node.H = H(cell, goal);
            node.F = node.G + node.H;
        }
    }
}