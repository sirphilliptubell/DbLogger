using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DbLogger.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Removes non alpha numeric characters.
        /// </summary>D:\Repositories\DbLogger\DbLogger.Lib\Extensions\StringExtensions.cs
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string ExceptNonAlphaNumeric(this string s)
            => new Regex("[^a-zA-Z0-9 -]").Replace(s, string.Empty);
    }
}
