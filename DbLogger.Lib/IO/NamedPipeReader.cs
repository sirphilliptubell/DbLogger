using System;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace DbLogger.IO
{
    /// <summary>
    /// Reads LogEntries from a DbLogger.NamedPipeServer on the local machine until disposed
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class NamedPipeReader : ILogReader
    {
        /// <summary>
        /// Occurs when a Reset was called on the logger.
        /// </summary>
        public event EventHandler OnReset;

        /// <summary>
        /// Occurs when a new Entry arrives from the server.
        /// </summary>
        public event EventHandler<LogEntry> OnEntry;

        private readonly Thread _pipeThread;
        private bool _continue = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeReader"/> class.
        /// </summary>
        public NamedPipeReader()
        {            
            _pipeThread = new Thread(StartThread);
            _pipeThread.Start();
        }

        /// <summary>
        /// Starts the thread.
        /// </summary>
        private void StartThread()
        {
            while (_continue)
            {
                bool anythingRead = false;

                //reconnect after each message 
                using (var client = new NamedPipeClientStream(".", NamedPipeWriter.NAMED_PIPE_SERVER_NAME, PipeDirection.In, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Impersonation))
                {
                    client.Connect();

                    var formatter = new BinaryFormatter();

                    if (client.CanRead)
                    {
                        var obj = formatter.Deserialize(client);
                        var entry = (NamedPipeEntry)obj;

                        anythingRead = true;

                        if (entry.IsReset)
                            OnReset?.Invoke(this, EventArgs.Empty);
                        else
                            OnEntry?.Invoke(this, entry.LogEntry);
                    }
                }                

                //if data was read, immediately try to read more
                if (!anythingRead)
                    Thread.Sleep(100);
            }
        }

        public void Dispose()
        {
            _continue = false;
            _pipeThread.Abort();
        }
    }
}
