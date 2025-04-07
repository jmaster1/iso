using System;
using Common.TimeNS;
using Iso.Cells;
using Iso.Movables;
using NUnit.Framework;

namespace Iso.Tests
{
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
            Assert.NotNull(b1);
            Assert.AreEqual(b1.Cell, c1);
            Assert.IsFalse(b1.Moving);


            var ok = b1.MoveTo(3, 3);
            Assert.IsTrue(ok);

            var t = Movables.Time = new Time();
            Movables.Start();
            var dt = TimeSpan.FromMilliseconds(50);

            while (t.Value.Second < 8)
            {
                t.Update(dt);
            }
            Assert.IsTrue(b1.Cell.Is(3, 3));
        }
    }
}
