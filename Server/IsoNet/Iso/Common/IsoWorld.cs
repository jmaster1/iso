using Iso.Buildings;
using Iso.Cells;
using Iso.Movables;
using IsoNet.Core;

namespace IsoNet.Iso.Common;

public class IsoWorld : LogAware
{
    public readonly Cells Cells = new();
        
    public readonly Buildings Buildings = new();
        
    public readonly Movables Movables = new();
}
