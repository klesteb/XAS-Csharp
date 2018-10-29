
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;

using ServiceSupervisor.Configuration.Extensions;

namespace ServiceSupervisor.Configuration.Loaders {
    
    public class ConfigFile: XAS.App.Configuration.Loaders.ConfigFile {

        private readonly ILogger log = null;

        public ConfigFile(IErrorHandler handler, ILoggerFactory logFactory, IniFile iniFile): base(handler, logFactory, iniFile) {

            this.log = logFactory.Create(typeof(ConfigFile));

        }

        public override void Load(IConfiguration config) {

            base.Load(config);

            var key = config.Key;
            var section = config.Section;

            config.CreateSection(section.Web());

            if (iniFile.DoesSectionExist(section.Web())) {

                string[] keys = iniFile.GetSectionKeys(section.Web());

                foreach (string item in keys) {

                    string value = iniFile.GetValue(section.Web(), item);

                    switch (item.ToLower()) {
                        case "address":
                            config.UpdateKey(section.Web(), key.Address(), value);
                            break;

                        case "enable-client-certificates":
                            config.UpdateKey(section.Web(), key.EnableClientCertificates(), value);
                            break;

                        case "root-path":
                            config.UpdateKey(section.Web(), key.WebRootPath(), value);
                            break;

                    }

                }

            }

        }

    }

}
