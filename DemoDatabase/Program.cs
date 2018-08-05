using System;
using System.IO;
using System.Collections.Generic;

using XAS.App;
using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Alerting;
using XAS.Core.Security;
using XAS.Core.Spooling;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

using DemoModel;

namespace DemoDatabase {

    public class Program {

        static Int32 Main(string[] args) {

            string model = "DemoDatabase";

            // poormans DI

            var key = new Key();
            var secure = new Secure();
            var section = new Section();

            // build the configuration

            var config = new XAS.Core.Configuration.Configuration(section, key);
            config.Build();

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

            var handler = new ErrorHandler(config, logFactory);
            handler.SendMessage += new SendMessage(alerter.Send);

            // build the database access

            var dbm = new DBM(config, handler, logFactory);
            var initializer = new Initializer(dbm);
            var context = new DemoModel.Context(initializer, model);
            var repository = new DemoModel.Repositories(config, handler, logFactory, context);
            var manager = new Manager(context, repository);

            // run the application

            var app = new App(config, handler, logFactory, secure, manager);
            return app.Run(args);

        }

    }

    public class App: XAS.App.Shell {

        private readonly ILogger log = null;
        private readonly IManager manager = null;

        public App(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity secure, IManager manager):
            base(config, handler, logFactory, secure) {

            this.manager = manager;
            this.log = logFactory.Create(typeof(App));

        }

        public override Int32 RunApp(String[] args) {

            bool debug = config.GetValue(config.Section.Environment(), config.Key.Debug()).ToBoolean();
            var command = new Commands(config, handler, logFactory, manager);
            this.Commands = new CommandOptions(config, logFactory);

            this.Commands.Add("set", "set global settings", command.Set);
            this.Commands.Add("show", "show global settings", command.Show);
            this.Commands.Add("add", "add a new dinosaur", command.Add);
            this.Commands.Add("remove", "remove a dinosaur", command.Remove);
            this.Commands.Add("update", "update a dinosaur", command.Update);

            if (debug) {

                config.Dump();

            }

            return base.RunApp(args);

        }

        public override String GetUsage() {

            return "Usage: DemoDatabase\n   or: DemoDatabase --help";

        }

        public override String[] GetManual() {

            List<string> text = new List<string>();
            List<string> options = GetOptionsText();

            text.Add("");
            text.Add("NAME");
            text.Add("");
            text.Add("    DemoDatabasse - testing a database schema");
            text.Add("");
            text.Add("SYNPOSIS");
            text.Add("");
            text.Add("    DemoDatabase ");
            text.Add("");
            text.Add("    DemoDatabase [--help] [--manual] [--version]");
            text.Add("");
            text.Add("DESCRIPTION");
            text.Add("");
            text.Add("    Manipulate dinosaurs.");
            text.Add("");
            text.Add("OPTIONS and ARGUMENTS");
            text.Add("");

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

