using System;

using XAS.Core.Spooling;
using XAS.Core.Extensions;
using XAS.Core.Configuration;

namespace XAS.Core.Logging {

    /// <summary>
    /// A factory class to load loggers.
    /// </summary>
    /// 
    public class Factory: ILoggerFactory {

        private IConfiguration config = null;
        private Factory<LogType, Logger> factory = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// 
        /// The "type" is used to initialize the logger for a specific class. This is usually
        /// done in the following manner:
        /// 
        /// <code>
        /// 
        ///     public class Example {
        ///     
        ///         private readonly ILogger log = null;
        ///     
        ///         public Example(ILogFactory logFactory) {
        ///         
        ///             log = logFactory.Create(typeof(Example));
        ///      
        ///         }
        ///         
        ///         public void Method() {
        ///         
        ///             log.Info("this works)";
        ///             
        ///         }
        ///         
        ///     }
        ///     
        /// </code>
        /// 
        /// </remarks>
        /// 
        public Factory(IConfiguration config, ISpooler spooler) {

            this.config = config;
            this.factory = new Factory<LogType, Logger>();

            factory.Register(LogType.File, new Func<Logger>(() => new Loggers.File(config, spooler)));
            factory.Register(LogType.Json, new Func<Logger>(() => new Loggers.Json(config, spooler)));
            factory.Register(LogType.Event, new Func<Logger>(() => new Loggers.Event(config, spooler)));
            factory.Register(LogType.Console, new Func<Logger>(() => new Loggers.Console(config, spooler)));

        }

        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <param name="type">The LogType of the logger.</param>
        /// <param name="objType">The class type.</param>
        /// <returns>An initialized logger.</returns>
        /// 
        public Logger Create(Type objType) {

            var key = config.Key;
            var section = config.Section;
            string logType = config.GetValue(section.Environment(), key.LogType(), "console");
            string logLevel = config.GetValue(section.Environment(), key.LogLevel(), "info");

            var logger = factory.Create(logType.ToLogType())
                .SetType(objType)
                .SetLevel(logLevel.ToLogLevel());

            return logger;

        }

    }

}
