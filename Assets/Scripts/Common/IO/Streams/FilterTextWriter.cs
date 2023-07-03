using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Common.IO.Streams
{
    /// <summary>
    /// FilterTextWriter that delegates write to target
    /// </summary>
    public class FilterTextWriter : TextWriter
    {
        private readonly TextWriter target;

        public override Encoding Encoding => target.Encoding;
        
        public override IFormatProvider FormatProvider => target.FormatProvider;
        
        public override string NewLine {
            get => target.NewLine;
            set => target.NewLine = value;
        }

        public FilterTextWriter(TextWriter target)
        {
            this.target = target;
        }

        protected override void Dispose(bool disposing)
        {
            target.Dispose();
            base.Dispose(disposing);
        }

        public override void Close()
        {
            target.Close();
        }

        public override void Flush()
        {
            target.Flush();
        }

        public override Task FlushAsync()
        {
            return target.FlushAsync();
        }

        public override void Write(bool value)
        {
            target.Write(value);
        }

        public override void Write(char value)
        {
            target.Write(value);
        }

        public override void Write(char[] buffer)
        {
            target.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            target.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            target.Write(value);
        }

        public override void Write(double value)
        {
            target.Write(value);
        }

        public override void Write(int value)
        {
            target.Write(value);
        }

        public override void Write(long value)
        {
            target.Write(value);
        }

        public override void Write(object value)
        {
            target.Write(value);
        }

        public override void Write(float value)
        {
            target.Write(value);
        }

        public override void Write(string value)
        {
            target.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            target.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            target.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            target.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            target.Write(format, arg);
        }

        public override void Write(uint value)
        {
            target.Write(value);
        }

        public override void Write(ulong value)
        {
            target.Write(value);
        }

        public override Task WriteAsync(char value)
        {
            return target.WriteAsync(value);
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return target.WriteAsync(buffer, index, count);
        }

        public override Task WriteAsync(string value)
        {
            return target.WriteAsync(value);
        }

        public override void WriteLine()
        {
            target.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            target.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            target.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(string value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            target.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            target.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            target.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            target.WriteLine(format, arg);
        }

        public override void WriteLine(uint value)
        {
            target.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            target.WriteLine(value);
        }

        public override Task WriteLineAsync()
        {
            return target.WriteLineAsync();
        }

        public override Task WriteLineAsync(char value)
        {
            return target.WriteLineAsync(value);
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return target.WriteLineAsync(buffer, index, count);
        }

        public override Task WriteLineAsync(string value)
        {
            return target.WriteLineAsync(value);
        }

        public override object InitializeLifetimeService()
        {
            return target.InitializeLifetimeService();
        }

        public override string ToString()
        {
            return target.ToString();
        }
    }
}
