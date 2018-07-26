using System;
using System.Text.RegularExpressions;

using XAS.Core.Logging;
using XAS.Core.Extensions;
using XAS.Core.Configuration;

namespace XAS.Core.Exceptions {

    /// <summary>
    /// A delegate to send a simple message string.
    /// </summary>
    /// <param name="message">The string to send.</param>
    /// 
    public delegate void SendMessage(String message);

    /// <summary>
    /// General purpose error and exception handler.
    /// </summary>
    /// 
    public class ErrorHandler: IErrorHandler {

        private readonly bool alert = false;
        private readonly ILogger log = null;
        private readonly IConfiguration config = null;

        /// <summary>
        /// An event to send a error message.
        /// </summary>
        /// 
        public event SendMessage SendMessage = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public ErrorHandler(IConfiguration config, ILoggerFactory logFactory) {
        
            this.config = config;

            var key = config.Key;
            var section = config.Section;

            log = logFactory.Create(typeof(ErrorHandler));
            alert = config.GetValue(section.Environment(), key.Alerts(), "false").ToBoolean();

        }

        /// <summary>
        /// Log a error based on the exception.
        /// </summary>
        /// <param name="ex">An exception.</param>
        /// 
        public void Errors(Exception ex) {

            string message = String.Format("{0}: {1}", ex.GetType().FullName, ex.Message);

            log.Fatal(message);

            if (ex.InnerException != null) {

                log.Fatal(String.Format("  InnerException: {0}: {1}", ex.InnerException.GetType().FullName, ex.InnerException.Message));

            }

            if (ex.GetType().ToString() == "AggregateException") {

                var aggregate = (AggregateException)ex;

                foreach (var e in aggregate.InnerExceptions) {

                    log.Fatal(String.Format("    InnerException: {0}: {1}", ex.GetType().FullName, ex.Message));

                    var x = e.InnerException;

                    while (x != null) {

                        log.Fatal(String.Format("    InnerException: {0}: {1}", x.GetType().FullName, x.Message));

                        x = x.InnerException;

                    }

                }

            }

        }

        /// <summary>
        /// Log a fatal error based on the exception.
        /// </summary>
        /// <param name="ex">An exception.</param>
        /// <remarks>
        /// This method will also send an alert.
        /// </remarks>
        /// 
        public void Exceptions(Exception ex) {

            PrintException(ex);
            SendAlert(ex);

        }

        /// <summary>
        /// Log a fatal error based on the exception.
        /// </summary>
        /// <param name="ex">An exception.</param>
        /// <returns></returns>
        /// <remarks>
        /// This method will also send an alert and return a exit code.
        /// </remarks>
        /// 
        public Int32 Exit(Exception ex) {

            int rc = 1;

            PrintException(ex);
            SendAlert(ex);

            return rc;

        }

        #region Private Methods

        private void SendAlert(Exception ex) {

            var key = config.Key;
            var section = config.Section;
            string script = config.GetValue(section.Environment(), key.Script());

            if (alert) {

                if (SendMessage != null) {

                    string message = String.Format("{0} -- {1}: {2}", script, ex.GetType().FullName, ex.Message);

                    SendMessage(message);

                }

            }

        }

        private void PrintException(Exception ex) {

            string message = String.Format("{0}: {1}", ex.GetType().FullName, ex.Message);

            log.Fatal(message);

            if (ex.InnerException != null) {

                log.Fatal(String.Format("  InnerException: {0}: {1}", ex.InnerException.GetType().FullName, ex.InnerException.Message));
                PrintStackTrace(ex.InnerException, 4);

            }

            if (ex.GetType().ToString() == "AggregateException") {

                var aggregate = (AggregateException)ex;

                foreach (var e in aggregate.InnerExceptions) {

                    log.Fatal(String.Format("  InnerException: {0}: {1}", ex.GetType().FullName, ex.Message));
                    PrintStackTrace(ex, 4);

                    var x = e.InnerException;

                    while (x != null) {

                        log.Fatal(String.Format("  InnerException: {0}: {1}", x.GetType().FullName, x.Message));
                        PrintStackTrace(x, 4);

                        x = x.InnerException;

                    }

                }

            }

        }

        private void PrintStackTrace(Exception ex, int offset) {

            string substitution = @" ";
            string pattern = @"---.*---";
            string padding = new string(' ', offset);

            RegexOptions options = RegexOptions.Multiline;
            Regex regex = new Regex(pattern, options);

            // taken from http://www.eqqon.com/index.php/Navigating_in_Exception_Stack_Traces_in_Visual_C-Sharp
            // with modifications

            foreach (string line in ex.StackTrace.Split(new string[] { " at " }, StringSplitOptions.RemoveEmptyEntries)) {

                if (string.IsNullOrEmpty(line.Trim()))
                    continue;

                string newLine = regex.Replace(line, substitution);

                string[] parts;
                parts = newLine.Trim().Split(new string[] { " in " }, StringSplitOptions.RemoveEmptyEntries);
                string class_info = parts[0].Trim();

                if (parts.Length == 2) {

                    parts = parts[1].Trim().Split(new string[] { "line" }, StringSplitOptions.RemoveEmptyEntries);
                    string src_file = parts[0];
                    int line_nr = int.Parse(parts[1]);

                    string output = String.Format("{0}at {1} in {2}: line {3}", padding, class_info, src_file.TrimEnd(':'), line_nr);
                    log.Fatal(output);

                } else {

                    string output = String.Format("{0}at {1}", padding, class_info);
                    log.Fatal(output);

                }

            }

        }

        #endregion

    }

}
