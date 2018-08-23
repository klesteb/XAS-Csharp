using System;
using System.Text.RegularExpressions;

namespace XAS.Core.Extensions {
    
    /// <summary>
    /// DateTime extensions
    /// </summary>
    /// 
    public static class DateTimeExtensions {

        private static string replacement = @"$1";
        private static string pattern = @"(-\d\d):";

        /// <summary>
        /// Return an ISO8601 formatted DateTime string.
        /// </summary>
        /// <remarks>
        /// Returns a string formatted like this: 2018-04-05T11:19:47.817-0700
        /// </remarks>
        /// <param name="dt">A DateTime object.</param>
        /// <returns>A formatted string.</returns>
        /// 
        public static String ToISO8601(this DateTime dt) {

            return Regex.Replace(dt.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), pattern, replacement);

        }

        /// <summary>
        /// Convert UNIX epoch to DateTime.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns>A DateTime object.</returns>
        /// 
        public static DateTime FromUnixTime(this Int64 unixTime) {

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);

        }

        /// <summary>
        /// Convert a DateTime to a UNIX epoch (64bit).
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Number of seconds from 1970-01-01 00:00:00.</returns>
        /// 
        public static Int64 ToUnixTime(this DateTime date) {

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);

        }

    }

}
