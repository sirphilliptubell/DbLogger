using DbLogger.IO;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;

namespace DbLogger
{
    public class Logger : IDisposable
    {
        private static Logger _instance;
        private static object _syncRoot = new object();

        /// <summary>
        /// Gets the singleton instance of Logger.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new Logger();
                    }
                }

                return _instance;
            }
        }

        private readonly Func<ILogEntryBuilder> _builderFactory;
        private readonly Dictionary<string, ILogEntryBuilder> _builders = new Dictionary<string, ILogEntryBuilder>();
        private readonly Dictionary<string, TextWriterRedirector> _redirectors = new Dictionary<string, TextWriterRedirector>();
        private readonly List<ILogWriter> _writers = new List<ILogWriter>();

        private Logger(Func<ILogEntryBuilder> builderFactory = null)
        {
            _builderFactory = builderFactory;
        }

        /// <summary>
        /// Gets the object which creates entries from lines of text.
        /// </summary>
        /// <param name="sourceName">Name of the source.</param>
        /// <returns></returns>
        private ILogEntryBuilder GetBuilder(string sourceName)
        {
            var key = sourceName.ToLowerInvariant();

            if (!_builders.ContainsKey(key))
            {
                var builder = _builderFactory?.Invoke();
                if (builder == null)
                    builder = new LogEntryBuilder(sourceName);

                //when the entry is ready, forward the entry to be logged
                builder.EntryComplete += Builder_EntryComplete;

                _builders[key] = builder;
            }

            return _builders[key];
        }

        /// <summary>
        /// Forwards the log entries to any writers that have been specified.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="entry">The entry.</param>
        private void Builder_EntryComplete(ILogEntryBuilder source, LogEntry entry)
        {
            foreach (var writer in _writers)
                writer.Write(entry);
        }

        /// <summary>
        /// Adds specified writer as a destination for the log entries.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">writer</exception>
        public void AddDestination(ILogWriter writer)
            => _writers.Add(writer ?? throw new ArgumentNullException(nameof(writer)));

        /// <summary>
        /// Adds the specified directory as a destination for the log entries.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public void AddDestination(DirectoryInfo directory)
            => AddDestination(new FileWriter(directory));

        /// <summary>
        /// Adds the builtin NamedPipe as a destination.
        /// </summary>
        public void AddNamedPipeDestination()
            => AddDestination(NamedPipeWriter.Instance);

        /// <summary>
        /// Tries to log to target.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="line">The log line.</param>
        private void TryToLogToTarget(string name, string line)
        {
            var builder = GetBuilder(name);
            builder.AddLine(line);
        }

        /// <summary>
        /// Gets the delegate to assign to the specified DbContext's Database.Log property.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <returns></returns>
        /// <remarks>
        /// Not limiting to inherit from DbContext as that would require a reference to EntityFramework.dll and the client 
        /// may not need that reference if only using a DataContext.
        /// </remarks>
        public Action<string> GetDbContextLogger<TDbContext>()
            => GetDbContextLogger(typeof(TDbContext).Name);

        /// <summary>
        /// Gets the delegate to assign to the DbContext's Database.Log property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public Action<string> GetDbContextLogger(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return line => TryToLogToTarget(name, line);
        }

        /// <summary>
        /// Configures the logging for the specified DataContext.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <returns></returns>
        public TextWriter AddToDataContext<TDataContext>(TDataContext dataContext)
            where TDataContext : DataContext
            => dataContext.Log = GetDataContextLogger(typeof(TDataContext).Name);

        /// <summary>
        /// Gets the TextWriter to use for a DataContext.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public TextWriter GetDataContextLogger(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!_redirectors.ContainsKey(name))
                _redirectors[name] = new TextWriterRedirector(line => TryToLogToTarget(name, line));

            return _redirectors[name];
        }

        public void Dispose()
        {
            //dispose the pipe server in case it's running
            NamedPipeWriter.Instance.Dispose();
        }
    }
}
