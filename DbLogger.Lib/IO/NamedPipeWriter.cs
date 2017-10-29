using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace DbLogger.IO
{
    /// <summary>
    /// A Named Pipe server that sends Log Entries to a client.
    /// </summary>
    /// <seealso cref="DbLogger.ILogWriter" />
    /// <seealso cref="System.IDisposable" />
    public class NamedPipeWriter : ILogWriter, IDisposable
    {
        public const string NAMED_PIPE_SERVER_NAME = "DbLogger.NamedPipeServer";

        private static object _syncRoot = new Object();
        private static NamedPipeWriter _instance;

        public static NamedPipeWriter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new NamedPipeWriter();
                    }
                }

                return _instance;
            }
        }
        
        private readonly BlockingCollection<NamedPipeEntry> _queue = new BlockingCollection<NamedPipeEntry>();
        private readonly Thread _pipeThread;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeWriter"/> class.
        /// </summary>
        public NamedPipeWriter()
        {
            _pipeThread = new Thread(StartThread);
            _pipeThread.Start();
        }

        /// <summary>
        /// Enqueues a "Reset" entry.
        /// </summary>
        public void Reset()
        {
            _queue.Add(NamedPipeEntry.Reset());
        }

        /// <summary>
        /// Enqueues a LogEntry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Write(LogEntry entry)
        {
            _queue.Add(NamedPipeEntry.Entry(entry));
        }

        /// <summary>
        /// Starts the thread that sends all queued items until this object is disposed.
        /// </summary>
        private void StartThread()
        {            
            try
            {
                foreach (var item in _queue.GetConsumingEnumerable(_tokenSource.Token))
                {
                    //restart the server with each item, the client may disconnect after the message is read
                    using (var server = new NamedPipeServerStream(NAMED_PIPE_SERVER_NAME, PipeDirection.Out, maxNumberOfServerInstances: 1, transmissionMode: PipeTransmissionMode.Byte))
                    {
                        //client may disconnect after each message, so try to reconnect if needed
                        if (!server.IsConnected)
                            server.WaitForConnection();

                        var formatter = new BinaryFormatter();
                        formatter.Serialize(server, item);
                        server.WaitForPipeDrain();
                        server.Close();
                    }

                }
            }
            catch (OperationCanceledException)
            { }
        }

        public void Dispose()
        {
            //try to give a little time for the queue to be flushed
            _tokenSource.CancelAfter(3000);
            Thread.Sleep(3100);
            //force queue to end.
            _queue.CompleteAdding();
            _queue.Dispose();
        }
    }
}
