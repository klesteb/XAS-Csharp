using System;
using System.IO;
using System.Collections.Generic;

using XAS.App;
using XAS.Network.TCP;
using XAS.Core.Logging;
using XAS.Core.Alerting;
using XAS.Core.Security;
using XAS.Core.Spooling;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

namespace DemoEchoClient {

    class Program {

        static Int32 Main(string[] args) {

            // poormans DI

            // build the configuration

            var config = new Configuration();
            config.Build();

            var key = config.Key;
            var section = config.Section;

            // build the locker

            var lockName = config.GetValue(config.Section.Environment(), config.Key.LockName(), "locked");
            var lockDriver = config.GetValue(config.Section.Environment(), config.Key.LockDriver()).ToLockDriver();
            var locker = new XAS.Core.Locking.Factory(lockName).Create(lockDriver);

            // build the alerter

            var spooler = new Spooler(config, locker);
            spooler.Directory = Path.Combine(spooler.Directory, "alerts");
            var logFactory = new XAS.Core.Logging.Factory(config, spooler);
            var alerter = new Alert(config, logFactory, spooler);

            // build the error handler

            var errorHandler = new ErrorHandler(config, logFactory);
            errorHandler.SendMessage += new SendMessage(alerter.Send);

            var secure = new Secure();

            // run the application

            var app = new App(config, errorHandler, logFactory, secure);
            return app.Run(args);

        }

        public class App: XAS.App.Shell {

            public Int32 Port { get; set; }
            public String Server { get; set; }

            public App(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity secure) :
                base(config, handler, logFactory, secure) {

            }

            public override Int32 RunApp(String[] args) {

                var client = new Client(config, handler, logFactory);

                client.Server = Server;
                client.Port = Port;

                var cmdHandler = new Commands(config, handler, logFactory, client);
                this.Commands = new CommandOptions(config, handler, logFactory);

                cmdHandler.Port = this.Port;
                cmdHandler.Server = this.Server;

                this.Commands.Add("send", "send test", cmdHandler.Send);
                this.Commands.Add("set", "set global settings", cmdHandler.Set);
                this.Commands.Add("show", "show global settings", cmdHandler.Show);

                return base.RunApp(args);

            }

            public override Options GetOptions() {

                this.Port = 7;
                this.Server = "127.0.0.1";

                var options = base.GetOptions();

                options.Add("port=", "the port of the server to use, defaults to \"7\"", (v) => {
                    this.Port = Convert.ToInt32(v);
                });

                options.Add("server=", "the server to connect too, defaults to \"localhost\".", (v) => {
                    this.Server = v;
                });

                return options;

            }

            public override String GetUsage() {

                return "Usage: demo-echo-client\n   or: demo-echo-client --help";

            }

            public override String[] GetManual() {

                var key = config.Key;
                var section = config.Section;
                List<string> text = new List<string>();
                List<string> options = this.GetOptionsText();
                string cfgFile = config.GetValue(section.Environment(), key.CfgFile());

                text.Add("");
                text.Add("NAME");
                text.Add("");
                text.Add("    demo-echo-client - interact with a echo server");
                text.Add("");
                text.Add("SYNPOSIS");
                text.Add("");
                text.Add("    demo-echo-client ");
                text.Add("");
                text.Add("    demo-echo-client [--help] [--manual] [--version]");
                text.Add("");
                text.Add("DESCRIPTION");
                text.Add("");
                text.Add("    This program will interact with a echo server.");
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
                text.Add("EXIT CODES");
                text.Add("");
                text.Add("    0 - success");
                text.Add("    1 - failure");
                text.Add("    2 - terminated");
                text.Add("");
                text.Add("SEE ALSO");
                text.Add("");
                text.Add("AUTHOR");
                text.Add("");
                text.Add("    Kevin L. Esteb - kesteb@wsipc.org");
                text.Add("");
                text.Add("COPYRIGHT AND LICENSE");
                text.Add("");
                text.Add("    Copyright (c) 2018 Kevin L. Esteb");
                text.Add("");
                text.Add("   This is free software you can redistribute it and/or modify it under");
                text.Add("   the terms of the Artistic License 2.0. For details, see the full text");
                text.Add("   of the license at http://www.perlfoundation.org/artistic_license_2_0.");
                text.Add("");

                return text.ToArray();

            }
        }

    }

}