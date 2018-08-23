
using XAS.Core;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;

using DemoMicroServiceClient.Configuration.Extensions;

namespace DemoMicroServiceClient.Configuration.Loaders {
    
    public class ConfigFile: XAS.App.Configuration.Loaders.ConfigFile {

        private readonly ILogger log = null;

        public ConfigFile(IErrorHandler handler, ILoggerFactory logFactory, IniFile iniFile): base(handler, logFactory, iniFile) {
         
            this.log = logFactory.Create(typeof(ConfigFile));

        }

        public override void Load(IConfiguration config) {

            log.Trace("Entering Load()");

            base.Load(config);

            var key = config.Key;
            var section = config.Section;

            config.CreateSection(section.Service());

            if (iniFile.DoesSectionExist(section.Service())) {

                string[] keys = iniFile.GetSectionKeys(section.Service());

                log.Debug(string.Format("keys: {0}", Utils.Dump(keys)));

                foreach (string item in keys) {

                    string value = iniFile.GetValue(section.Service(), item);

                    switch (item.ToLower()) {
                        case "server":
                            config.UpdateKey(section.Service(), key.Server(), value);
                            break;
                        case "port":
                            config.UpdateKey(section.Service(), key.Port(), value);
                            break;

                    }

                }

            }

            log.Trace("Leaving Load()");

        }

    }

}
