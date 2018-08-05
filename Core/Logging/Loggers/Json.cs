using System;
using System.IO;
using System.Text;

using log4net;
using log4net.Core;
using log4net.Layout;
using log4net.Appender;
using log4net.Repository.Hierarchy;

using XAS.Core.Spooling;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

namespace XAS.Core.Logging.Loggers {

    /// <summary>
    /// A class that implements a logger.
    /// </summary>
    /// 
    public class Json: Logger {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">A configuration object.</param>
        /// <param name="spooler">A spooler object.</param>
        /// 
        public Json(IConfiguration config, ISpooler spooler): base(config, spooler) { }

        /// <summary>
        /// This method confgures a default JSON based logger.
        /// </summary>
        /// 
        public override void Setup() {

            var key = config.Key;
            var section = config.Section;

            string pid = config.GetValue(section.Environment(), key.Pid());
            string hostname = config.GetValue(section.Environment(), key.Host());
            string script = config.GetValue(section.Environment(), key.Script());
            string priority = config.GetValue(section.Environment(), key.Priority());
            string facility = config.GetValue(section.Environment(), key.Facility());

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            XASLogstashLayout logstashLayout = new XASLogstashLayout();
            logstashLayout.Script = script;
            logstashLayout.ProcessID = pid;
            logstashLayout.Hostname = hostname;
            logstashLayout.Priority = priority;
            logstashLayout.Facility = facility;
            logstashLayout.ActivateOptions();

            XASSpoolAppender spoolAppender = new XASSpoolAppender();
            spoolAppender.Encoding = Encoding.UTF8;
            spoolAppender.Layout = logstashLayout;
            spoolAppender.Spooler = this.spooler;
            spoolAppender.ActivateOptions();

            hierarchy.Root.AddAppender(spoolAppender);
            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;

        }

    }

}
