using System.Text;
using Common.Editor;

namespace IsoNetTest.Iso.Info;

public class GenerateInfoTest : AbstractPlayerTest
{
    [Test]
    public void BuildInfoTest()
    {
        CommonUnityTasks.DataPathProvider = GetDataPath;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        CommonUnityTasks.BuildInfo();
    }

    public string GetDataPath()
    {
        var cd = Directory.GetCurrentDirectory();
        var dir = new DirectoryInfo(cd);

        while (dir != null)
        {
            var assetsPath = Path.Combine(dir.FullName, "Assets");
            if (Directory.Exists(assetsPath))
            {
                return assetsPath;
            }

            dir = dir.Parent;
        }
        throw new Exception("No directory found");
    }
}