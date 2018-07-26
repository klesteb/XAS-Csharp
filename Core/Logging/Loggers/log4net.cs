using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using log4net.Core;
using log4net.Util;

using Newtonsoft.Json;
using XAS.Core.Spooling;
using XAS.Core.Extensions;

namespace log4net.Ext.XAS {

    // taken from https://git-wip-us.apache.org/repos/asf?p=logging-log4net.git;a=tree;f=extensions/net/1.0/log4net.Ext.Trace;h=b87731d4704a50ee4761a4f450cb9be39733950a;hb=HEAD

    /// <summary>
    /// An extension of the log4net ILog interface to allow for a Trace logging level.
    /// </summary>
    /// 
    public interface IXASLog: ILog {

        /// <summary>
        /// Implement a Trace logging method.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// 
        void Trace(object message);

        /// <summary>
        /// Implement a Trace logging method that can write out an Exception.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="ex">An excecption to include with the log.</param>
        /// 
        void Trace(object message, Exception ex);

        /// <summary>
        /// Implement a Trace logging method with a format, and args for the foramt.
        /// </summary>
        /// <param name="format">A format to use when writing to the log.</param>
        /// <param name="args">The args used with the format.</param>
        /// 
        void TraceFormat(string format, params object[] args);

        /// <summary>
        /// Indicates wither the Trace level has been enabled/disabled.
        /// </summary>
        /// 
        bool IsTraceEnabled { get; }

    }

    /// <summary>
    /// A extended version of the log4net LogManager. This version supports a Trace log level.
    /// </summary>
    /// 
    public class XASLogManager {

        private static readonly WrapperMap s_wrapperMap = new WrapperMap(new WrapperCreationHandler(WrapperCreationHandler));

        private XASLogManager() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        public static IXASLog Exists(string name) {

            return Exists(Assembly.GetCallingAssembly(), name);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        public static IXASLog Exists(string domain, string name) {

            return WrapLogger(LoggerManager.Exists(domain, name));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        public static IXASLog Exists(Assembly assembly, string name) {

            return WrapLogger(LoggerManager.Exists(assembly, name));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public static IXASLog[] GetCurrentLoggers() {

            return GetCurrentLoggers(Assembly.GetCallingAssembly());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        /// 
        public static IXASLog[] GetCurrentLoggers(string domain) {

            return WrapLoggers(LoggerManager.GetCurrentLoggers(domain));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// 
        public static IXASLog[] GetCurrentLoggers(Assembly assembly) {

            return WrapLoggers(LoggerManager.GetCurrentLoggers(assembly));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        public static IXASLog GetLogger(string name) {

            return GetLogger(Assembly.GetCallingAssembly(), name);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        public static IXASLog GetLogger(string domain, string name) {

            return WrapLogger(LoggerManager.GetLogger(domain, name));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        public static IXASLog GetLogger(Assembly assembly, string name) {

            return WrapLogger(LoggerManager.GetLogger(assembly, name));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// 
        public static IXASLog GetLogger(Type type) {

            return GetLogger(Assembly.GetCallingAssembly(), type.FullName);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// 
        public static IXASLog GetLogger(string domain, Type type) {

            return WrapLogger(LoggerManager.GetLogger(domain, type));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// 
        public static IXASLog GetLogger(Assembly assembly, Type type) {

            return WrapLogger(LoggerManager.GetLogger(assembly, type));

        }

        private static IXASLog WrapLogger(ILogger logger) {

            return (IXASLog)s_wrapperMap.GetWrapper(logger);

        }

        private static IXASLog[] WrapLoggers(ILogger[] loggers) {

            IXASLog[] results = new IXASLog[loggers.Length];

            for (int i = 0; i < loggers.Length; i++) {

                results[i] = WrapLogger(loggers[i]);

            }

            return results;

        }

        private static ILoggerWrapper WrapperCreationHandler(ILogger logger) {


            return new XASLogImpl(logger);

        }

    }


    /// <summary>
    /// Define a log4net logger with Trace level support.
    /// </summary>
    /// 
    public class XASLogImpl: LogImpl,IXASLog {

        private readonly static Type ThisDeclaringType = typeof(XASLogImpl);
        private Level m_levelTrace;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">An ILogger object.</param>
        /// 
        public XASLogImpl(ILogger logger) : base(logger) { }

        /// <summary>
        /// Reload the logging levels.
        /// </summary>
        /// <param name="repository">An ILoggerRepository object.</param>
        /// 
        protected override void ReloadLevels(Repository.ILoggerRepository repository) {

            base.ReloadLevels(repository);
            m_levelTrace = repository.LevelMap.LookupWithDefault(Level.Trace);

        }

        /// <summary>
        /// Gets wither trace level is enabled.
        /// </summary>
        /// 
        public bool IsTraceEnabled {

            get { return Logger.IsEnabledFor(m_levelTrace); }

        }

        /// <summary>
        /// Send a trace message to the logger.
        /// </summary>
        /// <param name="message">A message.</param>
        /// 
        public void Trace(object message) {

            Logger.Log(
                ThisDeclaringType,
                 m_levelTrace,
                 message,
                 null
             );

        }

        /// <summary>
        /// Send a trace message to the logger.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="exception">The exception object.</param>
        /// 
        public void Trace(object message, Exception exception) {

            Logger.Log(
                ThisDeclaringType,
                m_levelTrace,
                message,
                exception
            );

        }

        /// <summary>
        /// Send a trace message to the logger.
        /// </summary>
        /// <param name="format">A formatted string.</param>
        /// <param name="args">Arguments for the format.</param>
        /// 
        public void TraceFormat(string format, params object[] args) {

            if (IsTraceEnabled) {

                Logger.Log(
                    ThisDeclaringType,
                    m_levelTrace,
                    new SystemStringFormat(CultureInfo.InvariantCulture, format, args),
                    null
                );

            }

        }

        /// <summary>
        /// Send a trace message to the logger.
        /// </summary>
        /// <param name="format">A formatted string.</param>
        /// <param name="arg0">The first argument for the format.</param>
        /// <param name="arg1">The second argument for the format.</param>
        /// 
        public void TraceFormat(string format, object arg0, object arg1) {

            if (IsTraceEnabled) {

                Logger.Log(
                    ThisDeclaringType,
                    m_levelTrace,
                    new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }),
                    null
                );

            }

        }

        /// <summary>
        /// Send a trace message to the logger.
        /// </summary>
        /// <param name="format">A formatted string.</param>
        /// <param name="arg0">The first argument for the format.</param>
        /// <param name="arg1">The second argument for the format.</param>
        /// <param name="arg2">The third argument for the format.</param>
        /// 
        public void TraceFormat(string format, object arg0, object arg1, object arg2) {

            if (IsTraceEnabled) {

                Logger.Log(
                    ThisDeclaringType,
                    m_levelTrace,
                    new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }),
                    null
                );

            }

        }

        /// <summary>
        /// Send a trace message to the logger.
        /// </summary>
        /// <param name="provider">An IFromatProvider.</param>
        /// <param name="format">A formatted string.</param>
        /// <param name="args">Arguments for the format.</param>
        /// 
        public void TraceFormat(IFormatProvider provider, string format, params object[] args) {

            if (IsTraceEnabled) {

                Logger.Log(
                    ThisDeclaringType,
                    m_levelTrace,
                    new SystemStringFormat(provider, format, args),
                    null
                );

            }

        }

    }

}

namespace log4net.Layout {

    /// <summary>
    /// A log4net layout class. This one provides a layout for the WPM logstash JSON format.
    /// </summary>
    /// 
    public class XASLogstashLayout: LayoutSkeleton {

        private string _priority = "low";
        private string _script = "changeme";
        private string _logType = "xas-logs";
        private string _facility = "changeme";
        private string _hostName = System.Environment.MachineName.ToLower();
        private string _processID = Process.GetCurrentProcess().Id.ToString();

        /// <summary>
        /// Get/Sets the hostname.
        /// </summary>
        /// 
        public String Hostname {
            get { return _hostName; }
            set { _hostName = value; }
        }

        /// <summary>
        /// Get/Sets the logging facility.
        /// </summary>
        /// 
        public string Facility {
            get { return _facility; }
            set { _facility = value; }
        }

        /// <summary>
        /// Get/Sets the log type.
        /// </summary>
        /// 
        public string LogType {
            get { return _logType; }
            set { _logType = value; }
        }

        /// <summary>
        /// Get/Sets the process id.
        /// </summary>
        /// 
        public String ProcessID {
            get { return _processID; }
            set { _processID = value; }
        }

        /// <summary>
        /// Get/Sets the logging priority.
        /// </summary>
        /// 
        public string Priority {
            get { return _priority; }
            set { _priority = value; }
        }

        /// <summary>
        /// Get/Sets the script name associated with this logging event.
        /// </summary>
        /// 
        public string Script {
            get { return _script; }
            set { _script = value; }
        }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// 
        public override void ActivateOptions() { }

        /// <summary>
        /// Format the JSON data strucure for this logging event.
        /// </summary>
        /// <param name="writer">The writter to use for output.</param>
        /// <param name="e">The logging event.</param>
        /// 
        public override void Format(TextWriter writer, LoggingEvent e) {

            StringBuilder sb = new StringBuilder();
            string format = "[{0}] {1,-5} - {2}";
            string msgStamp = e.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
            string timeStamp = e.TimeStamp.ToISO8601();

            sb.AppendFormat(format, msgStamp, e.Level, e.RenderedMessage);

            var data = new Dictionary<string, object> {
                ["@timestamp"] = timeStamp,
                ["@version"] = 1,
                ["@message"] = sb.ToString(),
                ["type"] = this.LogType,
                ["message"] = e.RenderedMessage,
                ["hostname"] = this.Hostname,
                ["priority"] = this.Priority,
                ["facility"] = this.Facility,
                ["logger"] = e.LoggerName,
                ["process"] = this.Script,
                ["tid"] = e.ThreadName,
                ["pid"] = this.ProcessID
            };

            writer.Write(JsonConvert.SerializeObject(data));

        }

    }

}

namespace log4net.Appender {

    /// <summary>
    /// A log4net appender class that uses the XAS spooler to write spool files.
    /// </summary>
    /// 
    public class XASSpoolAppender: AppenderSkeleton {

        private StringWriter _sw = null;
        private String _directory = null;
        private Encoding _encoder = null;
        private ISpooler _spooler = null;

        /// <summary>
        /// Gets/Sets the spool directory.
        /// </summary>
        /// 
        public String Directory {
            get { return _directory; }
            set { _directory = value; }
        }

        /// <summary>
        /// Gets/Sets the encoding.
        /// </summary>
        /// 
        public Encoding Encoding {
            get { return _encoder; }
            set { _encoder = value; }
        }

        /// <summary>
        /// Get/Set the spooler to handle the spool files.
        /// </summary>
        /// 
        public ISpooler Spooler {
            get { return _spooler;  }
            set { _spooler = value;  }
        }

        /// <summary>
        /// Return wither this class needs a layout.
        /// </summary>
        /// 
        protected override Boolean RequiresLayout {
            get { return true; }
        }
       
        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// 
        public override void ActivateOptions() {

            base.ActivateOptions();

            _sw = new StringWriter();

        }

        /// <summary>
        /// Write logging events to spool files.
        /// </summary>
        /// <param name="loggingEvent"></param>
        /// 
        protected override void Append(LoggingEvent loggingEvent) {

            this.Layout.Format(_sw, loggingEvent);
            byte[] packet = this.Encoding.GetBytes(_sw.ToString());

            _spooler.Write(packet);
            _sw.Flush();

        }

    }

}
