using Common.Api.Pool;
using Common.Lang;

namespace Common.Util.Math.Path.AStar
{
    public interface INodeList<TC> : IClearable where TC : class
    {
        bool IsEmpty();
        
        Node<TC> GetNode(TC cell);
        
        void Remove(Node<TC> node);
        
        Node<TC> BestNode();
        
        bool Contains(TC cell);
        
        void Add(Node<TC> node);
    
        void SetF(Node<TC> node, int newF);
    
        void Reset(Pool<Node<TC>> nodePool);	
    }
}