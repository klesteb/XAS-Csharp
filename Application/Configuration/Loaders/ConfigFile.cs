using System.IO;

using XAS.Core;
using XAS.Core.Logging;
using XAS.App.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;
using XAS.App.Configuration.Extensions;
using XAS.Core.Configuration.Extensions;

namespace XAS.App.Configuration.Loaders {
    
    /// <summary>
    /// Class to load a configuration file. 
    /// </summary>
    /// <remarks>
    /// 
    /// This class will allow you to load a configuration file to override specific items 
    /// within the environment. By default this file is located in %XAS_ETC% and is named 
    /// after the executable with a .ini extension. These items can futher be refined from 
    /// the command line. This is an example.
    /// 
    /// [application]
    /// alerts = true                - boolean value
    /// facility = systems           - free form text
    /// priority = low               - free form text
    /// trace = false                - boolean value
    /// debug = false                - boolean value
    /// log-type = file              - may be console, file, json, event
    /// log-file = <filename>        - a file name
    /// log-conf = <filename>        - a file name
    ///  
    /// This makes it fairly trivial to reconfigure a service and can also be used to set defaults 
    /// for command line applications.
    /// 
    /// </remarks>
    /// 
    public class ConfigFile: ILoader {

        private ILogger log = null;

        protected readonly IniFile iniFile = null;
        protected readonly IErrorHandler handler = null;
        protected readonly ILoggerFactory logFactory = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public ConfigFile(IErrorHandler handler, ILoggerFactory logFactory, IniFile iniFile) {

            this.handler = handler;
            this.iniFile = iniFile;
            this.logFactory = logFactory;
            this.log = logFactory.Create(typeof(ConfigFile));

        }

        /// <summary>
        /// Load a configuration.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <remarks>An inheriting class should call base.Load(config) to propagate the load behavior.</remarks>
        /// 
        public virtual void Load(IConfiguration config) {

            log.Trace("Entering Load()");

            var key = config.Key;
            var section = config.Section;

            string configFile = config.GetValue(section.Environment(), key.CfgFile());

            iniFile.Load();

            if (iniFile.DoesSectionExist(section.Application())) {

                string[] keys = iniFile.GetSectionKeys(section.Application());

                log.Debug(string.Format("Keys: {0}", Utils.Dump(keys)));

                foreach (string item in keys) {

                    string value = iniFile.GetValue(section.Application(), item);

                    switch (item.ToLower()) {
                        case "alerts":
                            config.UpdateKey(section.Environment(), key.Alerts(), value);
                            break;

                        case "facility":
                            config.UpdateKey(section.Environment(), key.Facility(), value);
                            break;

                        case "priority":
                            config.UpdateKey(section.Environment(), key.Priority(), value);
                            break;

                        case "trace":
                            if (value.ToBoolean()) {
                                config.UpdateKey(section.Environment(), key.Trace(), "true");
                                config.UpdateKey(section.Environment(), key.Debug(), "true");
                                config.UpdateKey(section.Environment(), key.LogLevel(), LogLevel.Trace.ToString());
                                log.SetLevel(LogLevel.Trace);
                            }
                            break;

                        case "debug":
                            if (value.ToBoolean()) {
                                config.UpdateKey(section.Environment(), key.Debug(), "true");
                                config.UpdateKey(section.Environment(), key.LogLevel(), LogLevel.Debug.ToString());
                                log.SetLevel(LogLevel.Debug);
                            }
                            break;

                        case "log-type":
                            if ((value != "console") && (value != "file") && (value != "event") && (value != "json")) {
                                throw new InvalidConfigException("log-type must be either: console, file, event or json");
                            }
                            config.UpdateKey(section.Environment(), key.LogType(), value);
                            log.Close();
                            log = logFactory.Create(typeof(Base));
                            break;

                        case "log-file":
                            config.UpdateKey(section.Environment(), key.LogFile(), value);
                            config.UpdateKey(section.Environment(), key.LogType(), LogType.File.ToString().ToLower());
                            log.Close();
                            log = logFactory.Create(typeof(Base));
                            break;

                        case "log-conf":
                            if (File.Exists(value)) {
                                config.UpdateKey(section.Environment(), key.LogConf(), value);
                                log.Close();
                                log = logFactory.Create(typeof(Base));
                            } else {
                                string format = config.GetValue(section.Messages(), key.NoLogConf());
                                throw new ConfFileMissingException(string.Format(format, value));
                            }
                            break;

                    }

                }

            }

            log.Trace("Leaving Load()");

        }

    }

}
