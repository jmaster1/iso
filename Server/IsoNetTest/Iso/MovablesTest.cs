using Iso.Cells;
using Iso.Movables;

namespace IsoNetTest.Iso;

public class MovablesTest : AbstractPlayerTest
{
    [Test]
    public void MoveTest()
    {
        const int s = 4;
        Cells.Create(s, s);
        Cells.ForEachPos((x, y) => Cells.Set(x, y, CellType.Traversable));

        var bi = new MovableInfo
        {
            Id = "M1",
            Velocity = 1
        };

        var c1 = Cells.Get(0, 0);
        var b1 = Movables.Add(bi, c1);
        Assert.That(b1, Is.Not.Null);
        Assert.That(c1, Is.EqualTo(b1.Cell));
        Assert.IsFalse(b1.Moving);


        var ok = b1.MoveTo(3, 3);
        Assert.IsTrue(ok);

        UpdateTime(8);
        Assert.IsTrue(b1.Cell.Is(3, 3));
    }
}