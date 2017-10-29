using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DbLogger.Extensions;

namespace DbLogger.IO
{
    /// <summary>
    /// Writes logs to files.
    /// </summary>
    /// <seealso cref="DbLogger.ILogWriter" />
    public class FileWriter : ILogWriter
    {
        private readonly DirectoryInfo _directory;
        private bool _reset = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWriter"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <exception cref="System.ArgumentNullException">directory</exception>
        public FileWriter(DirectoryInfo directory)
        {
            _directory = directory ?? throw new ArgumentNullException(nameof(directory));

            if (!directory.Exists)
                directory.Create();
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="sourceName">The name of the log's source.</param>
        /// <param name="msg">The log message.</param>
        public void Write(LogEntry entry)
        {
            if (entry == null)
                return;
            
            var file = GetFile(entry.SourceName ?? "NoSourceName");

            //delete the file if reset was previously called
            if (_reset)
            {
                if (file.Exists)
                    file.Delete();
                _reset = false;
            }

            using (var sw = new StreamWriter(file.FullName, append: true))
            {
                sw.WriteLine(entry.Text);
                sw.WriteLine();
            }
        }

        private FileInfo GetFile(string name)
            => new FileInfo($"{_directory.FullName}\\{name.ExceptNonAlphaNumeric()}.log");

        public void Reset()
            => _reset = true;            
    }
}
