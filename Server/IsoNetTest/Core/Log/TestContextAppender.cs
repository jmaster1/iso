namespace IsoNetTest.Core.Log;

public class TestContextAppender : IAppender
{
    public static readonly TestContextAppender Instance = new();

    public void Append(string text)
    {
        TestContext.Progress.WriteLine(text);
        TestContext.Progress.Flush();
    }
}
