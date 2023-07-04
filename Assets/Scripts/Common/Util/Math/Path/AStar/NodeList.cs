using Common.Api.Pool;
using Common.Lang.Collections;

namespace Common.Util.Math.Path.AStar
{
    public class NodeList<TC> : INodeList<TC> where TC : class
    {
        /**
         * nodes mapped by cells
         */
        private Map<TC, Node<TC>> map = new();
        
        public void Clear()
        {
            map.Clear();
        }

        public bool IsEmpty()
        {
            return map.Count == 0;
        }

        public Node<TC> GetNode(TC cell)
        {
            return map.Get(cell);
        }

        public void Remove(Node<TC> node)
        {
            map.RemoveValue(node);
        }

        public Node<TC> BestNode()
        {
            Node<TC> result = null;
            foreach (var node in map.Values)
            {
                if(result == null) {
                    result = node;
                } else {
                    if(node.F < result.F) {
                        result = node;
                    }
                }
            }
            return result;
        }

        public bool Contains(TC cell)
        {
            return map.ContainsKey(cell);
        }

        public void Add(Node<TC> node)
        {
            map[node.Cell] = node;
        }

        public void SetF(Node<TC> node, int newF)
        {
            node.F = newF;
        }

        public void Reset(Pool<Node<TC>> nodePool)
        {
            foreach (var node in map.Values)
            {
                nodePool.Put(node);
            }
            map.Clear();
        }
    }
}