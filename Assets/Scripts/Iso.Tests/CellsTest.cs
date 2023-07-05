using Iso.Cells;
using NUnit.Framework;

namespace Iso.Tests
{
    public class CellsTest
    {
        [Test]
        public void FindPathTest()
        {
            var cells = new Cells.Cells();
            cells.Create(3, 3);
            cells.ForEachPos((x, y) => cells.Set(x, y, CellType.Traversable));
            var path = cells.FindPath(cells.Get(0, 0), cells.Get(2, 2));
            Assert.NotNull(path);
            Assert.AreEqual(path.Count, 5);
        }
    }
}
