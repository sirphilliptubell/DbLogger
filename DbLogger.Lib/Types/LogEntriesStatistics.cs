using System;
using System.Collections.Generic;
using System.Text;

namespace DbLogger
{
    public class LogEntriesStatistics
    {
        private readonly Dictionary<string, int> _queryCount = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _queryAndParamCount = new Dictionary<string, int>();

        public void Add(LogEntry entry)
        {
            var query = entry.Query;
            if (!_queryCount.ContainsKey(query))
                _queryCount[query] = 1;
            else
                _queryCount[query] += 1;

            var queryAndParams = entry.QueryAndParameters;
            if (!_queryAndParamCount.ContainsKey(queryAndParams))
                _queryAndParamCount[queryAndParams] = 1;
            else
                _queryAndParamCount[queryAndParams] += 1;
        }

        /// <summary>
        /// Gets the number of times the Query of the specified entry has been added to this instance.
        /// Does not include parameters.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public int GetQueryCount(LogEntry entry)
        {
            var query = entry.Query;
            if (_queryCount.ContainsKey(query))
                return _queryCount[query];
            else
                return 0;
        }

        /// <summary>
        /// Gets the number of times the Query and the same parameters have been added to this instance.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public int GetQueryAndParamCount(LogEntry entry)
        {
            var queryAndParams = entry.QueryAndParameters;
            if (_queryAndParamCount.ContainsKey(queryAndParams))
                return _queryAndParamCount[queryAndParams];
            else
                return 0;
        }

        /// <summary>
        /// Resets all statistics.
        /// </summary>
        public void Reset()
        {
            _queryCount.Clear();
            _queryAndParamCount.Clear();
        }
    }
}
