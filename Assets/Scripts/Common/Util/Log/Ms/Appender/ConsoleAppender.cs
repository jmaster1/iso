using System;

namespace IsoNet.Core.Log.Appender
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
