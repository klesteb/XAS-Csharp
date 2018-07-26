using System;

using log4net;
using log4net.Core;
using log4net.Layout;
using log4net.Appender;
using log4net.Repository.Hierarchy;

using XAS.Core.Spooling;
using XAS.Core.Extensions;
using XAS.Core.Configuration;

namespace XAS.Core.Logging.Loggers {

    /// <summary>
    /// A class that implements a logger.
    /// </summary>
    /// 
    public class Console: Logger {

        /// <summary>
        /// Contructor.
        /// </summary>
        /// 
        public Console(IConfiguration config, ISpooler spooler): base(config, spooler) { }

        /// <summary>
        /// This method confgures a default console logger.
        /// </summary>
        /// 
        public override void Setup() {

            var key = config.Key;
            var section = config.Section;

            bool debug = config.GetValue(section.Environment(), key.Debug()).ToBoolean();
            bool trace = config.GetValue(section.Environment(), key.Trace()).ToBoolean();

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout consoleLayout = new PatternLayout();
            consoleLayout.ConversionPattern = "%-5level - %message%newline";

            if (debug || trace) {

                consoleLayout.ConversionPattern = "%-5level - %logger: %message%newline";

            }

            consoleLayout.ActivateOptions();

            ConsoleAppender console = new ConsoleAppender();
            console.Target = "Console.Error";
            console.Layout = consoleLayout;
            console.ActivateOptions();

            hierarchy.Root.AddAppender(console);
            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;

        }

    }

}
