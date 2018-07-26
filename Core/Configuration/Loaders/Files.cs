using System;
using System.IO;
using System.Collections.Generic;

namespace XAS.Core.Configuration.Loaders {

    public class Files: ILoader {
        
        public void Load(IConfiguration config) {

            var key = config.Key;
            var section = config.Section;
            string cfgFile = config.GetValue(section.Application(), key.CfgFile());
            string homeDir = config.GetValue(section.Environment(), key.HomeDir());
            string filename = Path.GetFileName(cfgFile);
            string homeCfg = Path.Combine(homeDir, filename);

            // load the global configuration file

            if (File.Exists(cfgFile)) {

                var cfgs = new IniFile(cfgFile).Load();

                Merge(config, cfgs);

            }

            // load the local configuration file

            if (File.Exists(homeCfg)) {

                var cfgs = new IniFile(homeCfg).Load();

                Merge(config, cfgs);

            }

        }

        #region Private Methods

        private static void Merge(IConfiguration configs, IniFile cfg) {

            foreach (string section in cfg.GetSections()) {

                var keyValues = new List<KeyValues>();

                foreach (string key in cfg.GetSectionKeys(section)) {

                    var keyValue = new KeyValues {
                        Key = key,
                        Value = cfg.GetValue(section, key, "")
                    };

                    keyValues.Add(keyValue);

                }

                configs.MergeSection(section, keyValues);

            }

        }

        #endregion

    }

}
