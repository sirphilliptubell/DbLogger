using System;
using System.Collections.Generic;
using System.Text;

namespace DbLogger.IO
{
    /// <summary>
    /// Writes Db logs.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Writes the specified log entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        void Write(LogEntry entry);

        /// <summary>
        /// Resets the log.
        /// </summary>
        void Reset();
    }
}
