using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DbLogger.IO
{
    /// <summary>
    /// An item passed through the NamedPipe.
    /// </summary>
    [Serializable]
    public class NamedPipeEntry
    {
        /// <summary>
        /// Gets a value indicating whether a reset is passed through the pipe.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is reset; otherwise, <c>false</c>.
        /// </value>
        public bool IsReset { get; }

        /// <summary>
        /// Gets a value indicating whether this instance contains a LogEntry.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is entry; otherwise, <c>false</c>.
        /// </value>
        public bool IsEntry
            => LogEntry != null;

        /// <summary>
        /// Gets the entry, null if IsEntry is false.
        /// </summary>
        /// <value>
        /// The entry.
        /// </value>
        public LogEntry LogEntry { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeEntry"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private NamedPipeEntry(LogEntry entry)
        {
            this.LogEntry = entry;
            this.IsReset = entry == null;
        }

        /// <summary>
        /// Gets a new Reset Instance.
        /// </summary>
        /// <returns></returns>
        public static NamedPipeEntry Reset()
            => new NamedPipeEntry(null);

        /// <summary>
        /// Gets a new LogEntry instance.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">entry</exception>
        public static NamedPipeEntry Entry(LogEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return new NamedPipeEntry(entry);
        }
    }
}
