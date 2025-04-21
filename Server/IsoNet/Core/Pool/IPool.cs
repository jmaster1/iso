namespace IsoNet.Core.Pool;

public interface IPool<T> where T : new()
{
    T Get();

    void Return(T item);
}
