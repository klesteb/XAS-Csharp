using System;
using System.Text;
using System.Collections.Generic;

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

                throw new ArgumentException("The string must not be null or empty.");

            }

            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);

        }

        /// <summary>
        /// Convert a Base64 encode string.
        /// </summary>
        /// <param name="text">A Base64 encoded string object.</param>
        /// <returns>A decoded base64 string.</returns>
        /// 
        public static string FromBase64(this string text) {

            return FromBase64(text, Encoding.UTF8);

        }

        /// <summary>
        /// Convert a Base64 encoded string.
        /// </summary>
        /// <param name="text">A Base64 encode string object.</param>
        /// <param name="encoding">An Encoding object.</param>
        /// <returns>A decoded base64 string.</returns>
        /// 
        public static string FromBase64(this string text, Encoding encoding) {

            if (string.IsNullOrEmpty(text)) {

                throw new ArgumentException("The string must not be null or empty.");

            }

            byte[] textAsBytes = Convert.FromBase64String(text);
            return encoding.GetString(textAsBytes);

        }

        /// <summary>
        /// Returns with suffix removed, if present.
        /// </summary>
        /// 
        public static String TrimIfEndsWith(this String value, String suffix) {

            //taken from:  http://stackoverflow.com/questions/12936804/shorthand-way-to-remove-last-forward-slash-and-trailing-characters-from-string

            return
                value.EndsWith(suffix) ?
                    value.Substring(0, value.Length - suffix.Length) :
                    value;

        }

        /// <summary>
        /// Convert a string to a int32.
        /// </summary>
        /// <param name="value">A string representing a int32 value.</param>
        /// <returns>An Int32.</returns>
        /// 
        public static Int32 ToInt32(this String value) {

            return Convert.ToInt32(value);

        }

        /// <summary>
        /// Convert a string to a int64.
        /// </summary>
        /// <param name="value">A string representing a int64 value.</param>
        /// <returns>An Int64</returns>
        /// 
        public static Int64 ToInt64(this String value) {

            return Convert.ToInt64(value);

        }

        /// <summary>
        /// Convert a string to a single.
        /// </summary>
        /// <param name="value">A string representing a single value.</param>
        /// <returns>A Single.</returns>
        /// 
        public static Single ToSingle(this String value) {

            return Convert.ToSingle(value);

        }

        /// <summary>
        /// Convert a string to a double.
        /// </summary>
        /// <param name="value">A string representing a double value.</param>
        /// <returns>A Double.</returns>
        /// 
        public static Double ToDouble(this String value) {

            return Convert.ToDouble(value);

        }

        /// <summary>
        /// Parse a string of comma delimited numbers.
        /// </summary>
        /// <param name="buffer">A formatted string.</param>
        /// <returns>A list of Int32s.</returns>
        /// <remarks>
        /// The needed format of the string:
        ///    0,1,2
        /// </remarks>
        /// 
        public static List<Int32> ToInt32List(this String buffer) {

            var exitCodes = new List<Int32>();

            if (buffer != "") {

                String[] codes = buffer.Split(',');

                foreach (string code in codes) {

                    exitCodes.Add(code.ToInt32());

                }

            }

            return exitCodes;

        }

        /// <summary>
        /// Parse a string of key/value pairs.
        /// </summary>
        /// <param name="buffer">A formatted string.</param>
        /// <returns>A dictionary of key/value pairs.</returns>
        /// <remarks>
        /// The needed format of the string:
        ///     key=value;key=value;
        /// </remarks>
        /// 
        public static Dictionary<String, String> ToKeyValuePairs(this String buffer) {

            var keyValue = new Dictionary<String, String>();

            if (buffer != "") {

                String[] chunks = buffer.Split(';');

                foreach (string chunk in chunks) {

                    String[] parts = chunk.Split('=');

                    keyValue.Add(parts[0].Trim(), parts[1].Trim());

                }

            }

            return keyValue;

        }

    }

}
