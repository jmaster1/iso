using System;

namespace Common.Util.Log.Ms.Appender
{
    public class ConsoleAppender : IAppender
    {
        public static readonly ConsoleAppender Instance = new();
    
        public void Append(string text)
        {
            Console.WriteLine(text);
        }
    }
}
