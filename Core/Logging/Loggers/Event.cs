using System;

using log4net;
using log4net.Core;
using log4net.Layout;
using log4net.Appender;
using log4net.Repository.Hierarchy;

using XAS.Core.Spooling;
using XAS.Core.Configuration;

namespace XAS.Core.Logging.Loggers {

    /// <summary>
    /// A class that implements a logger.
    /// </summary>
    /// 
    public class Event: Logger {

        /// <summary>
        /// Construtor.
        /// </summary>
        /// 
        public Event(IConfiguration config, ISpooler spooler): base(config, spooler) { }

        /// <summary>
        /// This method confgures a default Event Log logger.
        /// </summary>
        /// 
        public override void Setup() {

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout eventLayout = new PatternLayout();
            eventLayout.ConversionPattern = "%-5level - %logger: %message";
            eventLayout.ActivateOptions();

            EventLogAppender eventLog = new EventLogAppender();
            eventLog.Layout = eventLayout;
            eventLog.ApplicationName = "Application";
            eventLog.LogName = "Application";
            eventLog.ActivateOptions();

            hierarchy.Root.AddAppender(eventLog);
            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;

        }

    }

}
