using System;
using System.Collections.Generic;
using System.Text;

namespace DbLogger
{
    /// <summary>
    /// Creates LogEntries from lines of text.
    /// </summary>
    /// <seealso cref="DbLogger.ILogEntryBuilder" />
    public class LogEntryBuilder : ILogEntryBuilder
    {
        private readonly List<string> _lines = new List<string>();
        
        /// <summary>
        /// Occurs when an entry is completed.
        /// </summary>
        public event LogEntryCreatedEventHandler EntryComplete;

        /// <summary>
        /// Gets the name of the log's source.
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryBuilder"/> class.
        /// </summary>
        /// <param name="sourceName">Name of the source.</param>
        /// <exception cref="System.ArgumentNullException">sourceName</exception>
        public LogEntryBuilder(string sourceName)
        {
            SourceName = sourceName ?? throw new ArgumentNullException(nameof(sourceName));
        }
        
        /// <summary>
        /// Adds a line to the entry.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void AddLine(string msg)
        {
            if (msg == Environment.NewLine)
            {
                //entry is complete
                var copy = _lines.ToArray();
                this.EntryComplete?.Invoke(this, new LogEntry(this.SourceName, copy));
                _lines.Clear();
            } else
            {
                _lines.Add(msg);
            }
        }
    }
}
