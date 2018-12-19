using System;
using System.IO;
using System.Collections.Generic;

using XAS.App;

using XAS.Core.Logging;
using XAS.Core.Alerting;
using XAS.Core.Security;
using XAS.Core.Spooling;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

namespace DemoShell {

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

            public App(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure) :
                base(config, errorHandler, logFactory, secure) {

            }

            public override Int32 RunApp(String[] args) {

                var handler = new Commands(config, logFactory);
                this.Commands = new CommandOptions(config, logFactory);

                handler.Port = this.Port;
                handler.Server = this.Server;

                this.Commands.Add("set", "set global settings", handler.Set);
                this.Commands.Add("show", "show global settings", handler.Show);
                this.Commands.Add("schedule", "schedule a job to run", handler.Schedule);

                return base.RunApp(args);

            }

            public override Options GetOptions() {

                this.Port = 9511;
                this.Server = "localhost";

                var options = base.GetOptions();

                options.Add("port=", "the port of the server to use, defaults to \"9511\"", (v) => {
                    this.Port = Convert.ToInt32(v);
                });

                options.Add("server=", "the server to connect too, defaults to \"localhost\".", (v) => {
                    this.Server = v;
                });

                return options;

            }

            public override String GetUsage() {

                return "Usage: DemoShell\n   or: DemoShell --help";

            }

            public override String[] GetManual() {

                List<string> text = new List<string>();
                List<string> options = this.GetOptionsText();

                text.Add("");
                text.Add("NAME");
                text.Add("");
                text.Add("    DemoShell - a demo of a Shell interface");
                text.Add("");
                text.Add("SYNPOSIS");
                text.Add("");
                text.Add("    DemoShell ");
                text.Add("");
                text.Add("    DemoShell [--help] [--manual] [--version]");
                text.Add("");
                text.Add("DESCRIPTION");
                text.Add("");
                text.Add("    This program is a demo of a shell interface.");
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

                return text.ToArray();

            }
        }

    }

}