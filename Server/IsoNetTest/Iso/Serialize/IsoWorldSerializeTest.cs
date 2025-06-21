using Common.IO.FileSystem;
using Common.TimeNS;
using Iso.Cells;
using Iso.Serialize.Json;

namespace IsoNetTest.Iso.Serialize;

public class IsoWorldSerializeTest : AbstractPlayerTest
{
    [Test]
    public void SerializeTest()
    {
        IsoTestContext.InitContext();
        
        const int s = 4;
        Cells.Create(s, s);
        Cells.ForEachPos((x, y) => Cells.Set(x, y, CellType.Buildable));
        
        var bi = Buildings.BuildingInfoSet.Get(0);
        
        Buildings.Build(bi, 0, 0);
        Buildings.Build(bi, 2, 2, true);

        var t = new Time();
        World.Bind(t);
        t.UpdateSec(1);
        t.UpdateSec(1);
        
        var ser = new IsoWorldJsonSerializer(World);
        var fs = ser.SaveAll();
        var lfs = new LocalFileSystem("C:\\tmp\\x");
        fs.CopyTo(lfs);
        ser.Load(fs);
        
        Assert.AreEqual(2, Buildings.Count);
    }
}