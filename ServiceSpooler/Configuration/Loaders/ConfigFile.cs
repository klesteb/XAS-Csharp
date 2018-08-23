using System.Collections.Generic;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;
using XAS.Core.Configuration.Extensions;

using ServiceSpooler.Configuration.Extensions;

namespace ServiceSpooler.Configuration.Loaders {
    
    /// <summary>
    /// Load and parse a configuration file.
    /// </summary>
    /// 
    public class ConfigFile: XAS.App.Configuration.Loaders.ConfigFile {

        private readonly ILogger log = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// <param name="iniFile">A IniFile object.</param>
        /// 
        public ConfigFile(IErrorHandler handler, ILoggerFactory logFactory, IniFile iniFile): base(handler, logFactory, iniFile) {

            this.log = logFactory.Create(typeof(ConfigFile));

        }

        /// <summary>
        /// Load the configuration.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public override void Load(IConfiguration config) {

            base.Load(config);

            var key = config.Key;
            var section = config.Section;

            config.CreateSection(section.MessageQueue());

            if (iniFile.DoesSectionExist(section.MessageQueue())) {

                string[] keys = iniFile.GetSectionKeys(section.MessageQueue());

                foreach (string item in keys) {

                    string value = iniFile.GetValue(section.MessageQueue(), item);

                    switch (item.ToLower()) {
                        case "server":
                            config.UpdateKey(section.MessageQueue(), key.Server(), value);
                            break;

                        case "port":
                            config.UpdateKey(section.MessageQueue(), key.Port(), value);
                            break;

                        case "use-ssl":
                            config.UpdateKey(section.MessageQueue(), key.UseSSL(), value);
                            break;

                        case "username":
                            config.UpdateKey(section.MessageQueue(), key.Username(), value);
                            break;

                        case "password":
                            config.UpdateKey(section.MessageQueue(), key.Password(), value);
                            break;

                        case "keepalive":
                            config.UpdateKey(section.MessageQueue(), key.KeepAlive(), value);
                            break;

                        case "level":
                            config.UpdateKey(section.MessageQueue(), key.Level(), value);
                            break;

                    }

                }

            }

            List<string> directories = config.GetSections();

            foreach (string directory in directories) {

                if ((directory != section.Application()) && (directory != section.MessageQueue())) {

                    // check for duplicates, last one wins!!

                    if (! config.DoesSectionExist(directory)) {

                        config.CreateSection(directory);

                    }

                    string[] keys = iniFile.GetSectionKeys(directory);

                    foreach (string item in keys) {

                        string value = iniFile.GetValue(directory, item);

                        switch (item.ToLower()) {
                            case "queue":
                                config.UpdateKey(directory, key.Queue(), value);
                                break;

                            case "packet-type":
                                config.UpdateKey(directory, key.PacketType(), value);
                                break;

                            case "alias":
                                config.UpdateKey(directory, key.Alias(), value);
                                break;

                        }

                    }

                }

            }

        }

    }

}
