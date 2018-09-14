
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;

using DemoEchoServer.Configuration.Extensions;

namespace DemoEchoServer.Configuration.Loaders {
    
    public class ConfigFile: XAS.App.Configuration.Loaders.ConfigFile {

        private readonly ILogger log = null;

        public ConfigFile(IErrorHandler handler, ILoggerFactory logFactory, IniFile iniFile): base(handler, logFactory, iniFile) {

            this.log = logFactory.Create(typeof(ConfigFile));

        }

        public override void Load(IConfiguration config) {

            base.Load(config);

            var key = config.Key;
            var section = config.Section;

            config.CreateSection(section.SSL());
            config.CreateSection(section.Server());

            if (iniFile.DoesSectionExist(section.SSL())) {

                string[] keys = iniFile.GetSectionKeys(section.SSL());

                foreach (string item in keys) {

                    string value = iniFile.GetValue(section.SSL(), item);

                    switch (item.ToLower()) {
                        case "enable":
                            config.UpdateKey(section.SSL(), key.UseSSL(), value);
                            break;

                        case "certificate":
                            config.UpdateKey(section.SSL(), key.SSLCaCert(), value);
                            break;

                        case "verify-peer":
                            config.UpdateKey(section.SSL(), key.SSLVerifyPeer(), value);
                            break;

                        case "protocols":
                            config.UpdateKey(section.SSL(), key.SSLProtocols(), value);
                            break;

                    }

                }

            }

            if (iniFile.DoesSectionExist(section.Server())) {

                string[] keys = iniFile.GetSectionKeys(section.Server());

                foreach (string item in keys) {

                    string value = iniFile.GetValue(section.Server(), item);

                    switch (item.ToLower()) {
                        case "address":
                            config.UpdateKey(section.Server(), key.Address(), value);
                            break;

                        case "port":
                            config.UpdateKey(section.Server(), key.Port(), value);
                            break;

                        case "backlog":
                            config.UpdateKey(section.Server(), key.Backlog(), value);
                            break;

                        case "max-connections":
                            config.UpdateKey(section.Server(), key.MaxConnections(), value);
                            break;

                        case "client-timeout":
                            config.UpdateKey(section.Server(), key.ClientTimeout(), value);
                            break;

                        case "reaper-interval":
                            config.UpdateKey(section.Server(), key.ReaperInterval(), value);
                            break;

                    }

                }

            }

            if (log.IsDebugEnabled) {

                config.Dump();

            }

        }

    }

}
