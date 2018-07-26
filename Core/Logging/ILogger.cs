using System;

namespace XAS.Core.Logging {

    public interface ILogger {

        /// <summary>
        /// Get wither the info level is enabled.
        /// </summary>
        /// 
        Boolean IsInfoEnabled { get; }

        /// <summary>
        /// Gets wither the Warn level is enabled.
        /// </summary>
        /// 
        Boolean IsWarnEnabled { get; }

        /// <summary>
        /// Gets wither the Error level is enabled.
        /// </summary>
        /// 
        Boolean IsErrorEnabled { get; }

        /// <summary>
        /// Gets wither the Fatal level is enabled.
        /// </summary>
        /// 
        Boolean IsFatalEnabled { get; }

        /// <summary>
        /// Gets wither the Debug level is enabled.
        /// </summary>
        /// 
        Boolean IsDebugEnabled { get; }

        /// <summary>
        /// Gets wither the Trace level is enabled.
        /// </summary>
        /// 
        Boolean IsTraceEnabled { get; }

        /// <summary>
        /// Abstract method to do logger setup. This needs to be overridden.
        /// </summary>
        /// 
        void Setup();

        /// <summary>
        /// Close all loggers.
        /// </summary>
        /// 
        void Close();

        /// <summary>
        /// Set the logging level for all loggers.
        /// </summary>
        /// <param name="level">The desired level.</param>
        /// <returns>An instance of Logger.</returns>
        /// 
        Logger SetLevel(LogLevel level);

        /// <summary>
        /// Set log level type.
        /// </summary>
        /// <param name="type">An object type.</param>
        /// <returns>An instance of Logger.</returns>
        /// 
        Logger SetType(Type type);

        /// <summary>
        /// Send a debug message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        void Debug(String arg);

        /// <summary>
        /// Send a debug message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        void Debug(String arg, Exception ex);

        /// <summary>
        /// Send a debug message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The arg to send.</param>
        /// 
        void DebugMsg(String type, params object[] args);

        /// <summary>
        /// Send an error message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        void Error(String arg);

        /// <summary>
        /// Send an error message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        void Error(String arg, Exception ex);

        /// <summary>
        /// Send an error message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        void ErrorMsg(String type, params object[] args);

        /// <summary>
        /// Send a fatal message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        void Fatal(String arg);

        /// <summary>
        /// Send a fatal message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        void Fatal(String arg, Exception ex);

        /// <summary>
        /// Send a fatal message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        void FatalMsg(String type, params object[] args);

        /// <summary>
        /// Send an info message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        void Info(String arg);

        /// <summary>
        /// Send an info message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        void Info(String arg, Exception ex);

        /// <summary>
        /// Send an info message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        void InfoMsg(String type, params object[] args);

        /// <summary>
        /// Send a warn message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        void Warn(String arg);

        /// <summary>
        /// Send a warn message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        void Warn(String arg, Exception ex);

        /// <summary>
        /// Send a warn message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        void WarnMsg(String type, params object[] args);

        /// <summary>
        /// Send a trace message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        void Trace(String arg);

        /// <summary>
        /// Send a trace message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        void Trace(String arg, Exception ex);

        /// <summary>
        /// Send a trace message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        void TraceMsg(String type, params object[] args);

    }

}
