using System;
using System.IO;
using System.Collections.Generic;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Alerting;
using XAS.Core.Security;
using XAS.Core.Spooling;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;
using XAS.Core.Configuration.Extensions;

using DemoModel;

namespace DemoDatabaseInit {

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

            var secure = new Secure();

            // run the application

            var app = new App(config, errorHandler, logFactory, secure, loader);
            return app.Run(args);

        }

        public class App: XAS.App.Console {

            private ILoader configFile = null;

            public App(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure, ILoader loader) :
                base(config, errorHandler, logFactory, secure) {

                this.configFile = loader;

            }

            public override Int32 RunApp(String[] args) {

                Int32 rc = 0;
                string model = "DemoDatabase";

                var dbm = new DBM(config, handler, logFactory);
                var initializer = new Initializer(dbm);
                var context = new DemoModel.Context(initializer, model);
                var repository = new DemoModel.Repositories(config, handler, logFactory, context);
                var manager = new Manager(context, repository);

                using (var repo = manager.Repository as DemoModel.Repositories) {

                    var count = repo.Dinosaurs.Count();

                }

                return rc;

            }

            public override String GetUsage() {

                return "Usage: DemoDatabaseInit\n   or: DemoDatabaseInit --help";

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
                text.Add("    DemoDatabaseInit - Initialize the demo database");
                text.Add("");
                text.Add("SYNPOSIS");
                text.Add("");
                text.Add("    DemoDatabaseInit [--help] [--manual] [--version]");
                text.Add("");
                text.Add("DESCRIPTION");
                text.Add("");
                text.Add("    This program will initialize the demo database. By default, this will be");
                text.Add("    created in your %APPDATA% directory as a local SQL Server database. If you");
                text.Add("    want it somewhere else you need to create a blank database. If you want a ");
                text.Add("    different type of database you will need to modify the app.config and possible");
                text.Add("    recompile the code.");
                text.Add("");
                text.Add("OPTIONS and ARGUMENTS");
                text.Add("");
                text.Add("    Options:");

                foreach (var line in options) {

                    text.Add(String.Format("      {0}", line));
                }

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
