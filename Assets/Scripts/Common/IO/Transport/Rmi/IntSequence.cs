using System.Threading;

namespace Common.IO.Transport.Rmi
{
    public class IntSequence
    {
        private int _requestIdSeq;

        public IntSequence(int start = 0)
        {
            _requestIdSeq = start;
        }
    
        public int NextVal()
        {
            return Interlocked.Increment(ref _requestIdSeq);
        }
    }
}
