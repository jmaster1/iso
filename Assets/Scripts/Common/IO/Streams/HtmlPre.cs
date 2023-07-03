namespace Common.IO.Streams
{
    /// <summary>
    /// wrapper for string to be rendered in a pre tag
    /// </summary>
    public class HtmlPre
    {
        public string text;

        public HtmlPre(string text)
        {
            this.text = text;
        }
    }
}