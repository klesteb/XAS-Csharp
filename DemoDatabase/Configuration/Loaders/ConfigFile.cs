
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;

using DemoDatabase.Configuration.Extensions;

namespace DemoDatabase.Configuration.Loaders {
    
    public class ConfigFile: XAS.App.Configuration.Loaders.ConfigFile {

        private readonly ILogger log = null;

        public ConfigFile(IErrorHandler handler, ILoggerFactory logFactory, IniFile iniFile): base(handler, logFactory, iniFile) {
         
            this.log = logFactory.Create(typeof(ConfigFile));

        }

        public override void Load(IConfiguration config) {

            base.Load(config);

            var key = config.Key;
            var section = config.Section;

            config.CreateSection(section.Database());

            if (iniFile.DoesSectionExist(section.Database())) {

                string[] keys = iniFile.GetSectionKeys(section.Database());

                foreach (string item in keys) {

                    string value = iniFile.GetValue(section.Database(), item);

                    switch (item.ToLower()) {
                        case "model":
                            config.UpdateKey(section.Database(), key.DatabaseModel(), value);
                            break;

                    }

                }

            }

        }

    }

}
