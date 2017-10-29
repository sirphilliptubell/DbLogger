using System;
using System.Collections.Generic;
using System.Text;

namespace DbLogger.IO
{
    /// <summary>
    /// Forwards log messages to a delegate.
    /// </summary>
    /// <seealso cref="DbLogger.ILogWriter" />
    public class ActionWriter : ILogWriter
    {
        private readonly Action<string, string> _target;
        private readonly Action _reset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionWriter"/> class.
        /// </summary>
        /// <param name="logActionFor_sourceName_message">The delegate that will handle the log entry for the given Source Name and Log Message.</param>
        /// <exception cref="ArgumentNullException">logActionFor_sourceName_message</exception>
        public ActionWriter(Action<string, string> logActionFor_sourceName_message, Action optional_reset)
        {
            _target = logActionFor_sourceName_message ?? throw new ArgumentNullException(nameof(logActionFor_sourceName_message));
            _reset = optional_reset;
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="sourceName">The name of the log's source.</param>
        /// <param name="msg">The log message.</param>
        public void Write(LogEntry entry)
            => _target(entry.SourceName, entry.Text);

        /// <summary>
        /// Resets the log.
        /// </summary>
        public void Reset()
            => _reset?.Invoke();
    }
}
