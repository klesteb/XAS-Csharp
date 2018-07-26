using System;
using System.Text;
using XAS.Core.Locking;
using XAS.Core.Logging;

namespace XAS.Core.Extensions {

    /// <summary>
    /// String extensions.
    /// </summary>
    /// 
    public static class StringExtensions {

        /// <summary>
        /// Convert a string into a LogType.
        /// </summary>
        /// <param name="logType">A log type.</param>
        /// <returns>A LogType enum.</returns>
        /// 
        public static LogType ToLogType(this String logType) {

            LogType type;

            switch (logType.ToLower()) {
                case "file":
                    type = LogType.File;
                    break;
                case "json":
                    type = LogType.Json;
                    break;
                case "event":
                    type = LogType.Event;
                    break;
                default:
                    type = LogType.Console;
                    break;
            }

            return type;

        }

        /// <summary>
        /// Convert a string into a LogLevel
        /// </summary>
        /// <param name="logLevel">A log level.</param>
        /// <returns>Returns a LogLevel enum.</returns>
        /// 
        public static LogLevel ToLogLevel(this String logLevel) {

            LogLevel level;

            switch (logLevel.ToLower()) {
                case "warn":
                    level = LogLevel.Warn;
                    break;
                case "error":
                    level = LogLevel.Error;
                    break;
                case "fatal":
                    level = LogLevel.Fatal;
                    break;
                case "debug":
                    level = LogLevel.Debug;
                    break;
                case "trace":
                    level = LogLevel.Trace;
                    break;
                default:
                    level = LogLevel.Info;
                    break;
            }

            return level;

        }

        /// <summary>
        /// Convert a string to the appropiate LockDriver enum.
        /// </summary>
        /// <param name="lockDriver">A string value.</param>
        /// <returns>Returns a LockDriver enum.</returns>
        /// 
        public static LockDriver ToLockDriver(this String lockDriver) {

            LockDriver driver;

            switch (lockDriver.ToLower()) {
                case "mutex":
                    driver = LockDriver.Mutex;
                    break;
                default:
                    driver = LockDriver.Default;
                    break;
            }

            return driver;

        }

        /// <summary>
        /// Convert a string to a boolean value.
        /// </summary>
        /// <remarks>
        /// If the string is "true", "yes", "1" or "-1" the returned value is true.
        /// If the string is "false", "no" or "0" the returned value is false.
        /// 
        /// Defaults to false if the string doesn't match the above criteria.
        /// </remarks>
        /// <param name="value">A string value.</param>
        /// <returns>true/false</returns>
        ///
        public static Boolean ToBoolean(this String value) {

            bool stat;

            switch (value.ToLower()) {
                case "true":
                case "yes":
                case "1":
                case "-1":
                    stat = true;
                    break;

                case "false":
                case "no":
                case "0":
                    stat = false;
                    break;

                default:
                    stat = false;
                    break;
            }

            return stat;
        
        }
        
        /// <summary>
        /// Convert a string to a Base64 encoded string.
        /// </summary>
        /// <param name="text">A string object.</param>
        /// <returns>A Base64 encoded string.</returns>
        /// 
        public static string ToBase64(this string text) {

            return ToBase64(text, Encoding.UTF8);

        }

        /// <summary>
        /// Convert a string to a Base64 encoded string.
        /// </summary>
        /// <param name="text">A string object.</param>
        /// <param name="encoding">An Encoding object.</param>
        /// <returns>A Base64 encoded string.</returns>
        /// 
        public static string ToBase64(this string text, Encoding encoding) {

            if (string.IsNullOrEmpty(text)) {

                return text;

            }

            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);

        }

        /// <summary>
        /// Convert a Base64 encode string.
        /// </summary>
        /// <param name="text">A Base64 encoded string object.</param>
        /// <param name="decodedText">The resulting decoded string.</param>
        /// <returns>true if successful.</returns>
        /// 
        public static bool TryParseBase64(this string text, out string decodedText) {

            return TryParseBase64(text, Encoding.UTF8, out decodedText);

        }

        /// <summary>
        /// Convert a Base64 encoded string.
        /// </summary>
        /// <param name="text">A Base64 encode string object.</param>
        /// <param name="encoding">An Encoding object.</param>
        /// <param name="decodedText">The resulting decoded string.</param>
        /// <returns>true = if successful.</returns>
        /// 
        public static bool TryParseBase64(this string text, Encoding encoding, out string decodedText) {

            bool stat = false;

            decodedText = "";

            try {

                byte[] textAsBytes = Convert.FromBase64String(text);
                decodedText = encoding.GetString(textAsBytes);

                stat = true;

            } catch { }

            return stat;
        }

    }

}
