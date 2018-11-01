
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;
using XAS.Core.Configuration.Extensions;

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

            string[] processes = iniFile.GetSections();

            foreach (string process in processes) {

                if ((process != section.Application()) && (process != section.Web())) {

                    // check for duplicates, last one wins!!

                    if (!config.DoesSectionExist(process)) {

                        config.CreateSection(process);

                    }

                    string[] keys = iniFile.GetSectionKeys(process);

                    foreach (string item in keys) {

                        string value = iniFile.GetValue(process, item);

                        switch (item.ToLower()) {
                            case "command":
                                config.UpdateKey(process, key.SupervisorCommand(), value);
                                break;

                            case "domain":
                                config.GetValue(process, key.SupervisorDomain(), value);
                                break;

                            case "username":
                                config.UpdateKey(process, key.SupervisorUsername(), value);
                                break;

                            case "password":
                                config.UpdateKey(process, key.SupervisorPassword(), value);
                                break;

                            case "verb":
                                config.UpdateKey(process, key.SupervisorVerb(), value);
                                break;

                            case "auto-start":
                                config.UpdateKey(process, key.SupervisorAutoStart(), value);
                                break;

                            case "exit-retries":
                                config.UpdateKey(process, key.SupervisorExitRetries(), value);
                                break;

                            case "auto-restart":
                                config.UpdateKey(process, key.SupervisorAutoStart(), value);
                                break;

                            case "restart-delay":
                                config.UpdateKey(process, key.SupervisorRestartDelay(), value);
                                break;

                            case "exit-codes":
                                config.UpdateKey(process, key.SupervisorExitCodes(), value);
                                break;

                            case "working-directory":
                                config.UpdateKey(process, key.SupervisorWorkingDirectory(), value);
                                break;

                            case "environment":
                                config.UpdateKey(process, key.SupervisorEnvironment(), value);
                                break;

                        }

                    }

                }

            }

        }

    }

}
