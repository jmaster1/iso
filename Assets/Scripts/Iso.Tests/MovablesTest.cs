using System;
using Common.TimeNS;
using Iso.Cells;
using Iso.Movables;
using NUnit.Framework;

namespace Iso.Tests
{
    public class MovablesTest
    {
        [Test]
        public void MoveTest()
        {
            var cells = new Cells.Cells();
            var s = 4;
            cells.Create(s, s);
            cells.ForEachPos((x, y) => cells.Set(x, y, CellType.Traversable));

            var bi = new MovableInfo
            {
                Id = "M1",
                Velocity = 1
            };
            var movables = new Movables.Movables
            {
                Cells = cells
            };

            var c1 = cells.Get(0, 0);
            var b1 = movables.Add(bi, c1);
            Assert.NotNull(b1);
            Assert.AreEqual(b1.Cell, c1);
            Assert.IsFalse(b1.Moving);


            var ok = b1.MoveTo(3, 3);
            Assert.IsTrue(ok);

            var t = movables.Time = new Time();
            movables.Start();
            var dt = TimeSpan.FromMilliseconds(50);

            while (t.Value.Second < 8)
            {
                t.Update(dt);
            }
            Assert.IsTrue(b1.Cell.Is(3, 3));
        }
    }
}
