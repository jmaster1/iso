using Common.TimeNS;
using Iso.Player;

namespace IsoNetTest.Iso;

public class AbstractPlayerTest
{
    public readonly IsoWorld World = new();
    public global::Iso.Cells.Cells Cells => World.Cells;
    public global::Iso.Buildings.Buildings Buildings => World.Buildings;
    public global::Iso.Movables.Movables Movables => World.Movables;
        
    protected void UpdateTime(float seconds)
    {
        var t = new Time();
        World.Bind(t);
            
        var span = TimeSpan.FromSeconds(seconds);
        var dt = TimeSpan.FromMilliseconds(50);
        while (t.Value - DateTime.MinValue < span)
        {
            t.Update(dt);
        }
    }
}