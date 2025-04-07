using Iso.Buildings;
using Iso.Cells;
using NUnit.Framework;

namespace Iso.Tests
{
    public class BuildingsTest : AbstractPlayerTest
    {
        [Test]
        public void BuildTest()
        {
            const int s = 4;
            Cells.Create(s, s);
            Cells.ForEachPos((x, y) => Cells.Set(x, y, CellType.Buildable));

            var bi = new BuildingInfo
            {
                Id = "B1",
                width = 1,
                height = 2
            };

            var c1 = Cells.Get(0, 0);
            var b1 = Buildings.Build(bi, c1);
            Assert.NotNull(b1);
            Assert.AreEqual(b1.Cell, c1);
            Assert.IsFalse(b1.Flipped);
            Assert.AreEqual(b1.Width, bi.width);
            Assert.AreEqual(b1.Height, bi.height);
            b1.ForEachCell(c => Assert.AreEqual(c.Building, b1));
            
            var c2 = Cells.Get(2, 0);
            var b2 = Buildings.Build(bi, c2, true);
            Assert.NotNull(b2);
            Assert.AreEqual(b2.Cell, c2);
            Assert.IsTrue(b2.Flipped);
            Assert.AreEqual(b2.Width, bi.height);
            Assert.AreEqual(b2.Height, bi.width);
            b2.ForEachCell(c => Assert.AreEqual(c.Building, b2));
        }
    }
}
