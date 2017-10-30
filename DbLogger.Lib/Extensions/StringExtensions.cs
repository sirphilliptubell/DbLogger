using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gets the string that appears after the first found string.
        /// Returns null if none are found.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="searches">The searches.</param>
        /// <returns></returns>
        public static string AfterOrDefault(this string s, IEnumerable<string> searches)
        {
            if (s == null)
                return null;

            foreach (var search in searches)
            {
                var index = s.IndexOf(search);
                if (index != -1)
                    return s.Substring(index + search.Length, s.Length - search.Length + index);
            }
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether the string starts with any of the specified valus.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <param name="search">The start string to check.</param>
        /// <returns></returns>
        public static bool StartsWithAny(this string s, IEnumerable<string> search)
            => search
            .Any(x => s.StartsWith(x));

        /// <summary>
        /// Gets the string that appears after the first found string.
        /// Returns null if none are found.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="searches">The searches.</param>
        /// <returns></returns>
        public static string AfterOrDefault(this string s, string search)
        {
            if (s == null)
                return null;

            var index = s.IndexOf(search);
            if (index != -1)
                return s.Substring(index + search.Length, s.Length - search.Length + index);

            return null;
        }

        /// <summary>
        /// Gets the string that appears after the first found string.
        /// Returns null if none are found.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="searches">The searches.</param>
        /// <returns></returns>
        public static string BeforeOrDefault(this string s, string search)
        {
            if (s == null)
                return null;

            var index = s.IndexOf(search);
            if (index != -1)
                return s.Substring(0, index);

            return null;
        }

        /// <summary>
        /// Tries to convert the string to a DateTime.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns></returns>
        public static DateTime? AsDateTime(this string s)
            => DateTime.TryParse(s, out var value)
            ? value
            : default(DateTime?);

        /// <summary>
        /// Tries to convert the string to an Integer.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns></returns>
        public static int? AsInteger(this string s)
            => int.TryParse(s, out var value)
            ? value
            : default(int?);

        /// <summary>
        /// Splits the string into it's lines.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string[] SplitToLines(this string s)
            => s.Split(new string[] { "\r\n" }, StringSplitOptions.None);

        /// <summary>
        /// Splits the string into it's lines, removes empty lines.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string[] SplitToLinesExceptEmpty(this string s)
                    => s.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// Joins the lines with a NewLine separator.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns></returns>
        public static string JoinLines(this IEnumerable<string> lines)
            => string.Join(Environment.NewLine, lines);
    }
}
