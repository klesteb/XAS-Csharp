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

    }

}
