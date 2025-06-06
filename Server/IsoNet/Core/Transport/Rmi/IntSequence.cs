namespace IsoNet.Core.Transport.Rmi;

public class IntSequence(int start = 0)
{
    private int _requestIdSeq = start;
    
    public int NextVal()
    {
        return Interlocked.Increment(ref _requestIdSeq);
    }
}
