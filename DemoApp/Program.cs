using System;
using System.IO;
using System.Collections.Generic;

using XAS.App;
using XAS.Core.Logging;
using XAS.Core.Alerting;
using XAS.Core.Security;
using XAS.Core.Spooling;
using XAS.App.Exceptions;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.App.Configuration.Loaders;
using XAS.Core.Configuration.Loaders;
using XAS.Core.Configuration.Extensions;

namespace DemoApp {

    class Program {

        static Int32 Main(string[] args) {

            ILoader loader = null;

            // poormans DI

            var key = new Key();
            var secure = new Secure();
            var section = new Section();

            // build the configuration

            var config = new XAS.Core.Configuration.Configuration(section, key);
            config.Build();

            // build the locker

            var lockName = config.GetValue(section.Environment(), key.LockName(), "locked");
            var lockDriver = config.GetValue(section.Environment(), key.LockDriver()).ToLockDriver();
            var locker = new XAS.Core.Locking.Factory(lockName).Create(lockDriver);

            // build the alerter

            var spooler = new Spooler(config, locker);
            spooler.Directory = Path.Combine(spooler.Directory, "alerts");
            var logFactory = new XAS.Core.Logging.Factory(config, spooler);
            var alerter = new Alert(config, logFactory, spooler);

            // build the error handler

            var errorHandler = new ErrorHandler(config, logFactory);
            errorHandler.SendMessage += new SendMessage(alerter.Send);

            // load the optional config file

            string configFile = config.GetValue(section.Environment(), key.CfgFile());

            if (File.Exists(configFile)) {

                var iniFile = new IniFile(configFile);
                loader = new ConfigFile(errorHandler, logFactory, iniFile);
                
            }

            // run the application

            var app = new App(config, errorHandler, logFactory, secure, loader);
            return app.Run(args);

        }

        public class App: XAS.App.Console {

            private ILoader configFile = null;

            public App(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure, ILoader loader):
                base(config, errorHandler, logFactory, secure) {

                this.configFile = loader;

            }

            public override int RunApp(string[] args) {

                var key = config.Key;
                var section = config.Section;
                bool debug = config.GetValue(section.Environment(), key.Debug()).ToBoolean();

                if (debug) {

                    config.Dump();

                }

                return base.RunApp(args);

            }

            public override String GetUsage() {

                return "Usage: DemoApp\n   or: DemoApp --help";

            }

            public override Options GetOptions() {

                var key = config.Key;
                var section = config.Section;
                var options = base.GetOptions();

                string helpText = String.Format(
                    "use an alternative configuration file, default: \"{0}\"",
                    config.GetValue(section.Environment(), key.CfgFile())
                );

                options.Add("cfg-file=", helpText, (v) => {
                    if (File.Exists(v)) {
                        config.UpdateKey(section.Environment(), key.CfgFile(), v);
                        if (configFile != null) {
                            configFile.Load(config);
                        } else {
                            var iniFile = new IniFile(v);
                            configFile = new ConfigFile(handler, logFactory, iniFile);
                            configFile.Load(config);
                        }
                    } else {
                        string format = config.GetValue(section.Messages(), key.FileMissing());
                        throw new ConfigFileMissingException(String.Format(format, v));
                    }
                });

                return options;

            }

            public override String[] GetManual() {

                var key = config.Key;
                var section = config.Section;
                string cfgFile = config.GetValue(section.Environment(), key.CfgFile());
                List<string> text = new List<string>();
                List<string> options = GetOptionsText();

                text.Add("");
                text.Add("NAME");
                text.Add("");
                text.Add("    DemoApp - A demo application using XAS.App");
                text.Add("");
                text.Add("SYNPOSIS");
                text.Add("");
                text.Add("    DemoApp [--help] [--manual] [--version]");
                text.Add("");
                text.Add("DESCRIPTION");
                text.Add("");
                text.Add("    This program is a demo command line application using XAS.App.");
                text.Add("");
                text.Add("OPTIONS and ARGUMENTS");
                text.Add("");
                text.Add("    Options:");

                foreach (var line in options) {

                    text.Add(String.Format("      {0}", line));
                }

                text.Add("");
                text.Add("CONFIGURATION");
                text.Add("");
                text.Add("    The default configuration file is " + cfgFile + ", and contains the following stanzas:");
                text.Add("");
                text.Add("        [application]");
                text.Add("        alerts = true");
                text.Add("        facility = systems");
                text.Add("        priority = low");
                text.Add("        trace = false");
                text.Add("        debug = false");
                text.Add("        log-type = file");
                text.Add("        log-file = " + config.GetValue(section.Environment(), key.LogFile()));
                text.Add("        log-conf = " + config.GetValue(section.Environment(), key.LogConf()));
                text.Add("");
                text.Add("    This is the basic options that every program has, they can be overridden on the command line.");
                text.Add("    The above are the defaults and this stanza is not really needed. But it does allow you to easily");
                text.Add("    configure a service.");
                text.Add("");
                text.Add("EXIT CODES");
                text.Add("");
                text.Add("    0 - success");
                text.Add("    1 - failure");
                text.Add("");
                text.Add("SEE ALSO");
                text.Add("");
                text.Add("AUTHOR");
                text.Add("");
                text.Add("    Kevin L. Esteb - kevin@kesteb.us");
                text.Add("");
                text.Add("COPYRIGHT AND LICENSE");
                text.Add("");
                text.Add("    Copyright (c) 2018 Kevin L. Esteb");
                text.Add("");

                return text.ToArray();

            }

        }

    }

}
