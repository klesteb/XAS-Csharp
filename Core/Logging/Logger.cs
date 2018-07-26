using System;
using System.IO;

using log4net;
using log4net.Config;
using log4net.Ext.XAS;

using XAS.Core.Spooling;
using XAS.Core.Configuration;

namespace XAS.Core.Logging {

    /// <summary>
    /// Abstract Logger class.
    /// </summary>
    /// 
    public abstract class Logger: ILogger {

        private IXASLog logger = null;

        protected ISpooler spooler = null;
        protected IConfiguration config = null;

        /// <summary>
        /// Gets wither the Info level is enabled.
        /// </summary>
        /// 
        public Boolean IsInfoEnabled {
            get { return this.logger.IsInfoEnabled; }
        }

        /// <summary>
        /// Gets wither the Warn level is enabled.
        /// </summary>
        /// 
        public Boolean IsWarnEnabled {
            get { return this.logger.IsWarnEnabled; }
        }

        /// <summary>
        /// Gets wither the Error level is enabled.
        /// </summary>
        /// 
        public Boolean IsErrorEnabled {
            get { return this.logger.IsErrorEnabled; }
        }

        /// <summary>
        /// Gets wither the Fatal level is enabled.
        /// </summary>
        /// 
        public Boolean IsFatalEnabled {
            get { return this.logger.IsFatalEnabled; }
        }

        /// <summary>
        /// Gets wither the Debug level is enabled.
        /// </summary>
        /// 
        public Boolean IsDebugEnabled {
            get { return this.logger.IsDebugEnabled; }
        }

        /// <summary>
        /// Gets wither the Trace level is enabled.
        /// </summary>
        /// 
        public Boolean IsTraceEnabled {
            get { return this.logger.IsTraceEnabled; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Logger(IConfiguration configs, ISpooler spooler) {

            this.config = configs;
            this.spooler = spooler;

            var key = config.Key;
            var section = config.Section;

            string logConf = configs.GetValue(section.Environment(), key.LogConf());

            if (File.Exists(logConf)) {

                FileInfo file = new FileInfo(logConf);
                XmlConfigurator.ConfigureAndWatch(file);

            }

            if (!LogManager.GetRepository().Configured) {

                this.Setup();

            }

            this.logger = XASLogManager.GetLogger("root");

        }

        /// <summary>
        /// Abstract method to do logger setup. This needs to be overridden.
        /// </summary>
        /// 
        public abstract void Setup();

        /// <summary>
        /// Close all loggers.
        /// </summary>
        /// 
        public void Close() {

            this.logger.Logger.Repository.Shutdown();
            LogManager.GetRepository().Configured = false;

        }

        /// <summary>
        /// Set the logging type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>An instance of Logger</returns>
        /// 
        public Logger SetType(Type type) {

            this.logger = XASLogManager.GetLogger(type);

            return this;

        }

        /// <summary>
        /// Set the logging level for all loggers.
        /// </summary>
        /// <param name="level">The desired level.</param>
        /// <returns>An instance of Logger</returns>
        /// 
        public Logger SetLevel(LogLevel level) {

            string strLevel = level.ToString();

            log4net.Repository.ILoggerRepository[] repositories = log4net.LogManager.GetAllRepositories();

            // Configure all loggers to be at the specifiec level.

            foreach (log4net.Repository.ILoggerRepository repository in repositories) {

                repository.Threshold = repository.LevelMap[strLevel];
                log4net.Repository.Hierarchy.Hierarchy hierarchy = (log4net.Repository.Hierarchy.Hierarchy)repository;
                log4net.Core.ILogger[] loggers = hierarchy.GetCurrentLoggers();

                foreach (log4net.Core.ILogger logger in loggers) {

                    ((log4net.Repository.Hierarchy.Logger)logger).Level = hierarchy.LevelMap[strLevel];

                }

            }

            // Configure the root logger.

            log4net.Repository.Hierarchy.Hierarchy h = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            log4net.Repository.Hierarchy.Logger rootLogger = h.Root;
            rootLogger.Level = h.LevelMap[strLevel];

            return this;

        }

        /// <summary>
        /// Send a debug message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        public void Debug(String arg) {

            if (this.logger.IsDebugEnabled) {

                this.logger.Debug(arg);

            }

        }

        /// <summary>
        /// Send a debug message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        public void Debug(String arg, Exception ex) {

            if (this.logger.IsDebugEnabled) {

                this.logger.Debug(arg, ex);

            }

        }

        /// <summary>
        /// Send a debug message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The arg to send.</param>
        /// 
        public void DebugMsg(String type, params object[] args) {

            if (this.logger.IsDebugEnabled) {

                string output = Format(type, args);
                this.logger.Debug(output);

            }

        }

        /// <summary>
        /// Send an error message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        public void Error(String arg) {

            if (this.logger.IsErrorEnabled) {

                this.logger.Error(arg);

            }

        }

        /// <summary>
        /// Send an error message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        public void Error(String arg, Exception ex) {

            if (this.logger.IsErrorEnabled) {

                this.logger.Error(arg, ex);

            }

        }

        /// <summary>
        /// Send an error message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        public void ErrorMsg(String type, params object[] args) {

            if (this.logger.IsErrorEnabled) {

                string output = Format(type, args);
                this.logger.Error(output);

            }

        }

        /// <summary>
        /// Send a fatal message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        public void Fatal(String arg) {

            if (this.logger.IsFatalEnabled) {

                this.logger.Fatal(arg);

            }

        }

        /// <summary>
        /// Send a fatal message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        public void Fatal(String arg, Exception ex) {

            if (this.logger.IsFatalEnabled) {

                this.logger.Fatal(arg, ex);

            }

        }

        /// <summary>
        /// Send a fatal message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        public void FatalMsg(String type, params object[] args) {

            if (this.logger.IsFatalEnabled) {

                string output = Format(type, args);
                this.logger.Fatal(output);

            }

        }

        /// <summary>
        /// Send an info message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        public void Info(String arg) {

            if (this.logger.IsInfoEnabled) {

                this.logger.Info(arg);

            }

        }

        /// <summary>
        /// Send an info message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        public void Info(String arg, Exception ex) {

            if (this.logger.IsInfoEnabled) {

                this.logger.Info(arg, ex);

            }

        }

        /// <summary>
        /// Send an info message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        public void InfoMsg(String type, params object[] args) {

            if (this.logger.IsInfoEnabled) {

                string output = Format(type, args);
                this.logger.Info(output);

            }

        }

        /// <summary>
        /// Send a warn message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        public void Warn(String arg) {

            if (this.logger.IsWarnEnabled) {

                this.logger.Warn(arg);

            }

        }

        /// <summary>
        /// Send a warn message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        public void Warn(String arg, Exception ex) {

            if (this.logger.IsWarnEnabled) {

                this.logger.Warn(arg, ex);

            }

        }

        /// <summary>
        /// Send a warn message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        public void WarnMsg(String type, params object[] args) {

            if (this.logger.IsWarnEnabled) {

                string output = Format(type, args);
                this.logger.Warn(output);

            }

        }

        /// <summary>
        /// Send a trace message to the logger.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// 
        public void Trace(String arg) {

            if (this.logger.IsTraceEnabled) {

                this.logger.Trace(arg);

            }

        }

        /// <summary>
        /// Send a trace message to the logger, with an exception.
        /// </summary>
        /// <param name="arg">The message to send.</param>
        /// <param name="ex">The exception to user.</param>
        /// 
        public void Trace(String arg, Exception ex) {

            if (this.logger.IsTraceEnabled) {

                this.logger.Trace(arg, ex);

            }

        }

        /// <summary>
        /// Send a trace message to the logger, using a format string.
        /// </summary>
        /// <param name="type">The format to use.</param>
        /// <param name="args">The message to send.</param>
        /// 
        public void TraceMsg(String type, params object[] args) {

            if (this.logger.IsTraceEnabled) {

                string output = Format(type, args);
                this.logger.Trace(output);

            }

        }

        #region Private Methods

        private String Format(String type, params object[] args) {

            var section = config.Section;
            string format = config.GetValue(section.Messages(), type);

            return String.Format(format, args);

        }

        #endregion

    }

}
