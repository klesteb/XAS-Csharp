using System;
using System.Text;

using log4net;
using log4net.Core;
using log4net.Layout;
using log4net.Appender;
using log4net.Repository.Hierarchy;

using XAS.Core.Spooling;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

namespace XAS.Core.Logging.Loggers {

    /// <summary>
    /// A class that implements a logger.
    /// </summary>
    /// 
    public class File: Logger {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public File(IConfiguration config, ISpooler spooler): base(config, spooler) { }

        /// <summary>
        /// This method confgures a default file logger.
        /// </summary>
        /// 
        public override void Setup() {

            var key = config.Key;
            var section = config.Section;

            string logFile = config.GetValue(section.Environment(), key.LogFile());
            bool debug = config.GetValue(section.Environment(), key.Debug()).ToBoolean();
            bool trace = config.GetValue(section.Environment(), key.Trace()).ToBoolean();

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout fileLayout = new PatternLayout();
            fileLayout.ConversionPattern = "[%date{yyyy-MM-dd HH:mm:ss}] %-5level - %message%newline";

            if (debug || trace) {

                fileLayout.ConversionPattern = "[%date{yyyy-MM-dd HH:mm:ss}] %-5level - %logger: %message%newline";

            }

            fileLayout.ActivateOptions();

            FileAppender fileAppender = new FileAppender();
            fileAppender.Layout = fileLayout;
            fileAppender.AppendToFile = true;
            fileAppender.File = logFile;
            fileAppender.Encoding = Encoding.UTF8;
            fileAppender.ActivateOptions();

            hierarchy.Root.AddAppender(fileAppender);
            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;

        }

    }

}
