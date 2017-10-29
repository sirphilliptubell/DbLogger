using System;
using System.Collections.Generic;
using System.Text;

namespace DbLogger.IO
{
    /// <summary>
    /// Represents an object which reads log entries.
    /// </summary>
    public interface ILogReader : IDisposable
    {        
        /// <summary>
        /// Occurs when a Reset was called on the logger.
        /// </summary>
        event EventHandler OnReset;

        /// <summary>
        /// Occurs when a new Entry arrives from the server.
        /// </summary>
        event EventHandler<LogEntry> OnEntry;
    }
}
