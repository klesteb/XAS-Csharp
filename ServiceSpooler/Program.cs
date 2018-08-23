using System;
using System.IO;
using System.Collections.Generic;

using XAS.App;
using XAS.App.Exceptions;
using XAS.App.Configuration.Loaders;

using XAS.Core.Logging;
using XAS.Core.Alerting;
using XAS.Core.Security;
using XAS.Core.Spooling;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;
using XAS.Core.Configuration.Extensions;

namespace ServiceSpooler {

    class Program {

        static Int32 Main(string[] args) {

            ILoader loader = null;

            // poormans DI

            // build the configuration

            var config = new XAS.Core.Configuration.Configuration();
            config.Build();

            var key = config.Key;
            var section = config.Section;

            // build the locker

            var lockName = config.GetValue(section.Application(), key.LockName(), "locked");
            var lockDriver = config.GetValue(section.Application(), key.LockDriver()).ToLockDriver();
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
                loader.Load(config);

            }

            var secure = new Secure();

            // run the application

            var app = new App(config, errorHandler, logFactory, secure, loader);
            return app.Run(args);

        }

        public class App: XAS.App.Service {

            private ILoader configFile = null;
            private readonly ISecurity secure = null;

            public App(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure, ILoader loader) :
                base(config, errorHandler, logFactory, secure) {

                this.secure = secure;
                this.configFile = loader;

            }

            public override Int32 RunApp(String[] args) {

                this.WindowsService = new Service(config, handler, logFactory, secure);

                return base.RunApp(args);

            }

            public override String GetUsage() {

                return "Usage: xas-spoolerd\n   or: xas-spoolerd --help";

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
                List<string> text = new List<string>();
                List<string> options = GetOptionsText();
                string sbin = config.GetValue(section.Environment(), key.SbinDir());
                string cfgFile = config.GetValue(section.Environment(), key.CfgFile());
                string spoolDir = config.GetValue(section.Environment(), key.VarSpoolDir());

                text.Add("");
                text.Add("NAME");
                text.Add("");
                text.Add("    xas-spoolerd - The spooling service for the XAS Environment");
                text.Add("");
                text.Add("SYNPOSIS");
                text.Add("");
                text.Add("    sc start xas-spoolerd ");
                text.Add("");
                text.Add("    xas-spoolerd [--help] [--manual] [--version]");
                text.Add("");
                text.Add("DESCRIPTION");
                text.Add("");
                text.Add("    This program provides the spooling services for the XAS Environment. It does this");
                text.Add("    by reading a configuration file and monitoring the specified directories. If a file");
                text.Add("    is placed into one of those directories, it is forward to a message queue server.");
                text.Add("    Once the file has been passed to the message queue server, the spooler is then");
                text.Add("    notified and the local file is removed from the directory.");
                text.Add("");
                text.Add("    It communicates with the message queue server using the STOMP protocol. This is");
                text.Add("    a simple and portable protocol. There are libraries written for many languages");
                text.Add("    that can interact with this protocol.");
                text.Add("");
                text.Add("    This service can installed or deinstalled from the Service Control Manager by");
                text.Add("    doing the following:");
                text.Add("");
                text.Add("    Install");
                text.Add("        " + sbin + "\\xas-spoolerd --install");
                text.Add("");
                text.Add("    Deinstall");
                text.Add("         " + sbin + "\\xas-spoolerd --uninstall");
                text.Add("");
                text.Add("OPTIONS and ARGUMENTS");
                text.Add("");

                foreach (var line in options) {

                    text.Add(String.Format("    {0}", line));
                }

                text.Add("");
                text.Add("CONFIGURATION");
                text.Add("");
                text.Add("    The default configuration file is \"" + cfgFile + "\", and contains the following stanzas:");
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
                text.Add("        [message-queue]");
                text.Add("        server = " + config.GetValue(section.Environment(), key.MQServer()));
                text.Add("        port = " + config.GetValue(section.Environment(), key.MQPort()));
                text.Add("        use-ssl = false");
                text.Add("        username = guest");
                text.Add("        password = guest");
                text.Add("        keepalive = true");
                text.Add("        level = 1.0");
                text.Add("");
                text.Add("    This is for interaction with the message queue server. The above are the defaults. The level");
                text.Add("    can be between 1.0 - 1.2 and indicates the STOMP protocol level to use.");
                text.Add("");
                text.Add(String.Format("        [{0}]", Path.Combine(spoolDir, "alerts")));
                text.Add("        queue = /queue/alerts");
                text.Add("        packet-type = xas-alerts");
                text.Add("");
                text.Add(String.Format("        [{0}]", Path.Combine(spoolDir, "logs")));
                text.Add("        queue = /queue/logs");
                text.Add("        packet-type = xas-logs");
                text.Add("");
                text.Add("    This indicates which directories to monitor, the message queues to use as the destination endpoint");
                text.Add("    and the packet type. You can have as many as needed.");
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
