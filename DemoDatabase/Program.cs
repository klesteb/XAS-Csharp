using System;
using System.IO;
using System.Collections.Generic;

using XAS.App;
using XAS.App.Configuration;

using XAS.Core.Logging;
using XAS.Core.Alerting;
using XAS.Core.Security;
using XAS.Core.Spooling;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;

using XAS.Model;
using XAS.Model.Configuration;

using DemoDatabase.Model;
using DemoDatabase.Model.Service;
using DemoDatabase.Configuration;

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
            var context = new DemoDatabase.Model.Context(initializer, model);
            var repository = new DemoDatabase.Model.Repositories(config, handler, logFactory, context);
            var manager = new Manager(context, repository);

            // run the application

            var app = new App(config, handler, logFactory, secure, manager);
            return app.Run(args);

        }

    }

    public class App: XAS.App.Console {

        private readonly ILogger log = null;
        private readonly IManager manager = null;

        public string Site { get; set; }
        public string Group { get; set; }
        public bool Groups { get; set; }
        public string Target { get; set; }
        public bool Targets { get; set; }
        public string Server { get; set; }

        public App(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity secure, IManager manager):
            base(config, handler, logFactory, secure) {

            this.Groups = false;
            this.Targets = false;

            this.manager = manager;
            this.log = logFactory.Create(typeof(App));

        }


        public void PrintGroup(GroupDTO dto) {

            System.Console.WriteLine("------------------------------------------------");
            System.Console.WriteLine("Group: {0}", dto.Name);

            foreach (var target in dto.Targets) {

                System.Console.WriteLine("    Target: {0}", target.Name);

            }

        }

        public void PrintServer(ServerDTO dto) {

            System.Console.WriteLine("------------------------------------------------");
            System.Console.WriteLine("Server: {0}", dto.Name);

            foreach (var attribute in dto.Attributes) {

                System.Console.WriteLine("    Type: {0}, Name: {1}, Value: {2}",
                    attribute.Type,
                    attribute.Name,
                    (attribute.StrValue != null)
                            ? attribute.StrValue
                            : attribute.NumValue.ToString()
                );

            }

        }

        public void GetServer() {

            using (var repo = manager.Repository as Model.Repositories) {

                var dto = Services.GetServer(repo, this.Server);
                PrintServer(dto);

            }

        }

        public void GetServersByGroup() {

            using (var repo = manager.Repository as Model.Repositories) {

                var dtos = Services.GetServers(repo, this.Group);

                foreach (var dto in dtos) {

                    PrintServer(dto);

                }

            }

        }

        public void GetServers() {

            using (var repo = manager.Repository as Model.Repositories) {

                var dtos = Services.GetServers(repo, this.Group, this.Target);

                foreach (var dto in dtos) {

                    PrintServer(dto);

                }

            }

        }

        public void GetSites() {

            using (var repo = manager.Repository as Model.Repositories) {

                var dto = Services.GetSite(repo, this.Site);
                PrintServer(dto);

            }

        }

        public void GetGroups() {

            using (var repo = manager.Repository as Model.Repositories) {

                var dtos = Services.GetGroups(repo);

                foreach (var dto in dtos) {

                    PrintGroup(dto);

                }

            }

        }

        public void Setup() {

            // initialize the database, if needed

            var repo = manager.Repository as Model.Repositories;
            var count = repo.Servers.Count();

            // check the arguments

            if (!String.IsNullOrEmpty(Site)) {

                if (!String.IsNullOrEmpty(Group) || !String.IsNullOrEmpty(Target) ) { 

                    throw new ArgumentException("You can not specify --site along with --group or --target");

                }

                if (!String.IsNullOrEmpty(Server)) {

                    throw new ArgumentException("You can not specify --site along with --server");

                }

                if (Groups) {

                    throw new ArgumentException("You can not specify --groups along with --server");

                }

                return;

            }

            if (!String.IsNullOrEmpty(Server)) {

                if (!String.IsNullOrEmpty(Group) || !String.IsNullOrEmpty(Target)) {

                    throw new ArgumentException("You can not specify --server along with --group or --target");

                }

                if (!String.IsNullOrEmpty(Site)) {

                    throw new ArgumentException("You can not specify --server along with --site");

                }

                if (Groups) {

                    throw new ArgumentException("You can not specify --groups along with --site");

                }

                return;

            }

            if (Groups) {

                if (!String.IsNullOrEmpty(Group) || !String.IsNullOrEmpty(Target)) {

                    throw new ArgumentException("You can not specify --groups along with --group or --target");

                }

                if (!String.IsNullOrEmpty(Site)) {

                    throw new ArgumentException("You can not specify --server along with --groups");

                }

                if (!String.IsNullOrEmpty(Server)) {

                    throw new ArgumentException("You can not specify --server along with --groups");

                }
                
                return;

            }

            //if ((String.IsNullOrEmpty(Group) && !String.IsNullOrEmpty(Target)) ||
            //    (!String.IsNullOrEmpty(Group) && String.IsNullOrEmpty(Target))) {

            //    throw new ArgumentException("You must specify both --group and --target");

            //}

            if (String.IsNullOrEmpty(Site) && String.IsNullOrEmpty(Server) &&
                String.IsNullOrWhiteSpace(Group) && String.IsNullOrWhiteSpace(Target)) {

                DisplayUsage();

            }

        }

        public override Int32 RunApp(String[] args) {

            Int32 rc = 0;

            Setup();

            if (!String.IsNullOrEmpty(Site)) {

                GetSites();

            } else if (!String.IsNullOrEmpty(Server)) {

                GetServer();

            } else if (!String.IsNullOrEmpty(Group) && String.IsNullOrEmpty(Target)) {

                GetServersByGroup();

            } else if (this.Groups) {

                GetGroups();

            } else {

                GetServers();

            }

            return rc;

        }

        public override Options GetOptions() {

            Options options = base.GetOptions();

            options.Add("site=", "the site to connect too", (v) => {
                this.Site = v;
            });

            options.Add("group=", "the group to use for server selection", (v) => {
                this.Group = v;
            });

            options.Add("target=", "the target to use for server selection", (v) => {
                this.Target = v;
            });

            options.Add("server=", "the server to use", (v) => {
                this.Server = v;
            });

            options.Add("groups", "list the avaliable groups", (v) => {
                this.Groups = true;
            });

            return options;

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
            text.Add("    This program will initialze and test a database schema.");
            text.Add("");
            text.Add("    The current models are:");
            text.Add("");
            text.Add("        Master");
            text.Add("");
            text.Add("     Where \"Master\" is the default.");
            text.Add("");
            text.Add("OPTIONS and ARGUMENTS");
            text.Add("");
            text.Add("    Arguments:");
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
            text.Add("    Copyright (c) 2017 Kevin L. Esteb");
            text.Add("");

            return text.ToArray();

        }

    }

}

