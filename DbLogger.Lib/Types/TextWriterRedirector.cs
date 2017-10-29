using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DbLogger
{
    /// <summary>
    /// Redirects all writes to a given action.
    /// </summary>
    /// <seealso cref="System.IO.TextWriter" />
    internal class TextWriterRedirector
        : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        private readonly Action<string> _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterRedirector"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public TextWriterRedirector(Action<string> target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        private void Send(string msg)
            => _target(msg);

        private void SendLine(string msg)
            => _target(msg + Environment.NewLine);

        private void SendLine()
            => _target(Environment.NewLine);

        public override void Write(char value)
            => Send(value.ToString());

        public override void Write(bool value)
            => Send(value.ToString());

        public override void Write(char[] buffer)
            => Send(new string(buffer));

        public override void Write(char[] buffer, int index, int count)
            => Send(new string(buffer).Substring(index, count));

        public override void Write(decimal value)
            => Send(value.ToString());

        public override void Write(double value)
            => Send(value.ToString());

        public override void Write(float value)
            => Send(value.ToString());

        public override void Write(int value)
            => Send(value.ToString());

        public override void Write(long value)
            => Send(value.ToString());

        public override void Write(object value)
            => Send(value.ToString());

        public override void Write(string format, object arg0)
            => Send(string.Format(format, arg0));

        public override void Write(string format, object arg0, object arg1)
            => Send(string.Format(format, arg0, arg1));

        public override void Write(string format, object arg0, object arg1, object arg2)
            => Send(string.Format(format, arg0, arg1, arg2));

        public override void Write(string format, params object[] arg)
            => Send(string.Format(format, arg));

        public override void Write(string value)
            => Send(value);

        public override void Write(uint value)
            => Send(value.ToString());

        public override void Write(ulong value)
            => Send(value.ToString());

        public override void WriteLine()
            => SendLine();

        public override void WriteLine(bool value)
            => SendLine(value.ToString());

        public override void WriteLine(char value)
            => SendLine(value.ToString());

        public override void WriteLine(char[] buffer)
            => SendLine(new string(buffer));

        public override void WriteLine(char[] buffer, int index, int count)
            => SendLine(new string(buffer).Substring(index, count));

        public override void WriteLine(decimal value)
            => SendLine(value.ToString());

        public override void WriteLine(double value)
            => SendLine(value.ToString());

        public override void WriteLine(float value)
            => SendLine(value.ToString());

        public override void WriteLine(int value)
            => SendLine(value.ToString());

        public override void WriteLine(long value)
            => SendLine(value.ToString());

        public override void WriteLine(object value)
            => SendLine(value.ToString());

        public override void WriteLine(string format, object arg0)
            => SendLine(string.Format(format, arg0));

        public override void WriteLine(string format, object arg0, object arg1)
            => SendLine(string.Format(format, arg0, arg1));

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
            => SendLine(string.Format(format, arg0, arg1, arg2));

        public override void WriteLine(string format, params object[] arg)
            => SendLine(string.Format(format, arg));

        public override void WriteLine(string value)
            => SendLine(value);

        public override void WriteLine(uint value)
            => SendLine(value.ToString());

        public override void WriteLine(ulong value)
            => SendLine(value.ToString());

        public override Task WriteAsync(char value)
        {
            Send(value.ToString());
            return Task.FromResult(true);
        }

        public override Task WriteLineAsync()
        {
            SendLine();
            return Task.FromResult(true);
        }

        public override Task WriteLineAsync(char value)
        {
            SendLine(value.ToString());
            return Task.FromResult(true);
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            SendLine(new string(buffer).Substring(index, count));
            return Task.FromResult(true);
        }

        public override Task WriteLineAsync(string value)
        {
            SendLine(value);
            return Task.FromResult(true);
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            Send(new string(buffer).Substring(index, count));
            return Task.FromResult(true);
        }

        public override Task WriteAsync(string value)
        {
            Send(value);
            return Task.FromResult(true);
        }
    }
}
