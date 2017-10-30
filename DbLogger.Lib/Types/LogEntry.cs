using System;
using System.Collections.Generic;
using System.Linq;
using DbLogger.Extensions;

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
        /// Gets the date/time the query was run.
        /// </summary>
        public DateTime? DateTimeRun { get; }

        /// <summary>
        /// Gets the duration in milliseconds.
        /// </summary>
        public int? DurationInMilliseconds { get; }

        public string StackTrace { get; }

        /// <summary>
        /// Gets only the Query part of the log. Ingores all commented lines.
        /// </summary>
        public string Query
            => Text
            .SplitToLines()
            .Where(x => !x.StartsWith("--"))
            .JoinLines();

        /// <summary>
        /// Gets only the query and parameters used. Ingnores the timestamp/duration/execution/failure info, etc..
        /// </summary>
        public string QueryAndParameters
            => Text
            .SplitToLines()
            .Where(x => !x.StartsWithAny(new string[] { "-- Executing ", "--Completed in ", "-- Failed in ", "-- Canceled in " }))
            .JoinLines();

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

            this.DateTimeRun = GetTimeRun(this.Text);
            this.DurationInMilliseconds = GetDuration(this.Text);
            this.StackTrace = Environment.StackTrace
                .SplitToLines()
                //try to ignore stack frames we've created
                .SkipWhile(x => x.StartsWithAny(new string[] { "   at System.Environment", "   at DbLogger." }))
                .Skip(4) //skip a few extra, this should be improved, may be different for DataContext vs DbContext.
                .JoinLines();
        }

        public override string ToString()
        {
            if (DurationInMilliseconds.HasValue)
                return $"{SourceName} ({DurationInMilliseconds}) ms";
            else
                return SourceName;
        }

        private static DateTime? GetTimeRun(string text)
            //looking for a line like one of these:
            //-- Executing at 10/8/2013 10:55:41 AM -07:00
            //-- Executing asynchronously at 10/8/2013 10:55:41 AM -07:00
            => text
            .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Reverse()
            .FirstOrDefault(x => x.StartsWith("-- Executing "))
            .AfterOrDefault(new string[] { " at ", " asynchronously at " })
            .AsDateTime();

        private static int? GetDuration(string text)
            //looking for a line like this:
            //  "-- Completed in 12 ms with result: 1"
            //  "-- Failed in 1 ms with error: Invalid object name 'ThisTableIsMissing'."
            //  "-- Canceled in 1 ms"
            //see https://msdn.microsoft.com/en-us/library/dn469464%28v=vs.113%29.aspx?f=255&MSPPError=-2147217396
            => text
            .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Reverse()
            .FirstOrDefault(x => x.StartsWithAny(new string[] { "--Completed in ", "-- Failed in ", "-- Canceled in " }))
            .AfterOrDefault(new string[] { "--Completed in ", "-- Failed in ", "-- Canceled in " })
            .BeforeOrDefault(" ms")
            .AsInteger();
    }
}
