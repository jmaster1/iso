using Common.TimeNS;
using Iso.Player;

namespace IsoNetTest.Iso;

public class AbstractPlayerTest
{
    public readonly IsoPlayer Player = new();
    public global::Iso.Cells.Cells Cells => Player.Cells;
    public global::Iso.Buildings.Buildings Buildings => Player.Buildings;
    public global::Iso.Movables.Movables Movables => Player.Movables;
        
    protected void UpdateTime(float seconds)
    {
        var t = new Time();
        Player.Bind(t);
            
        var span = TimeSpan.FromSeconds(seconds);
        var dt = TimeSpan.FromMilliseconds(50);
        while (t.Value - DateTime.MinValue < span)
        {
            t.Update(dt);
        }
    }
}