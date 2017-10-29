using System;
using System.Collections.Generic;

namespace DbLogger
{
    /// <summary>
    /// An entry in the log.
    /// </summary>
    [Serializable]
    public class LogEntry
    {
        /// <summary>
        /// Gets the name of the source of the logs.
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// Gets the text of the entry.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public LogEntry(string sourceName, IEnumerable<string> text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            this.Text = string.Join(string.Empty, text);

            this.SourceName = sourceName ?? throw new ArgumentNullException(nameof(sourceName));
        }
    }
}
