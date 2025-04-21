using Microsoft.Extensions.Logging;

namespace IsoNet.Core;

public abstract class LogAware
{
    public ILogger? Logger;
    public void Clear()
    {
        throw new NotImplementedException();
    }
}
