using System;

namespace DbLogger
{
    public delegate void LogEntryCreatedEventHandler(ILogEntryBuilder source, LogEntry entry);

    /// <summary>
    /// Creates LogEntries from lines of text.
    /// </summary>
    public interface ILogEntryBuilder
    {
        /// <summary>
        /// Occurs when an entry completed.
        /// </summary>
        event LogEntryCreatedEventHandler EntryComplete;

        /// <summary>
        /// Adds a line to the entry.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        void AddLine(string msg);
    }
}
