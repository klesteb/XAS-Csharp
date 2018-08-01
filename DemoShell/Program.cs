using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using XAS.App;
using XAS.App.Exceptions;
using XAS.App.Configuration;

using XAS.Core.Logging;
using XAS.Core.Alerting;
using XAS.Core.Security;
using XAS.Core.Spooling;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;

namespace DemoShell {

    class Program {

        static Int32 Main(string[] args) {

            // poormans DI

            var key = new Key();
            var secure = new Secure();
            var section = new Section();

            // build the configuration

            var config = new Configuration(section, key);
            config.Build();

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

            // run the application

            var app = new App(config, errorHandler, logFactory, secure);
            return app.Run(args);

        }

        public class App: XAS.App.Shell {

            public Int32 Port { get; set; }
            public String Server { get; set; }

            public App(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure ): 
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

    public class Commands: CommandHandler {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly ILoggerFactory logFactory = null;

        public Int32 Port { get; set; }
        public String Server { get; set; }
        public String Requestor { get; set; }

        public Commands(IConfiguration config, ILoggerFactory logFactory): base() {

            this.config = config;
            this.logFactory = logFactory;

            this.log = logFactory.Create(typeof(Commands));
            this.Requestor = config.GetValue(config.Section.Environment(), config.Key.Username());

        }

        public Boolean Schedule(params String[] args) {

            bool displayHelp = false;
            string group = "production";
            string target = "production";
            string requestor = this.Requestor;
            string time = DateTime.Now.ToString("HH:mm") + ":00";
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            options.Add("requestor:", "the requestor of the job.", (v) => {
                requestor = v;
            });

            options.Add("date:", "the date to submit the job on, defaults to \"today\".", (v) => {
                if (v != "today") {
                    date = v;
                }
            });

            options.Add("time:", "the time to start the job, defaults to \"now\".", (v) => {
                if (v != "now") {
                    time = v;
                }
            });

            options.Add("group:", "the group to submit the job too, defaults to \"production\".", (v) => {
                group = v;
            });

            options.Add("target:", "the target to sumbit the job too, defaults to \"production\".", (v) => {
                target = v;
            });

            var parameters = options.Parse(args).ToArray(); // forces the options to be parsed

            if (displayHelp) {

                DisplayHelp("schedule --requestor <username> \"<job parameters>\"", options);

            } else {

                System.Console.WriteLine("requestor: {0}", requestor);
                System.Console.WriteLine("date     : {0}", date);
                System.Console.WriteLine("time     : {0}", time);
                System.Console.WriteLine("group    : {0}", group);
                System.Console.WriteLine("target   : {0}", target);

                foreach (var arg in parameters) {

                    System.Console.WriteLine("arg = {0}", arg);

                }

            }

            return true;
        
        }

        public Boolean Set(params String[] args) {

            bool displayHelp = false;
            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            options.Add("port=", "set the port number.", (v) => {
                this.Port = Convert.ToInt32(v);
            });

            options.Add("server=", "set the server name.", (v) => {
                this.Server = v;
            });

            options.Add("requestor=", "set the default requestor.", (v) => {
                this.Requestor = v;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("set", options);

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

        public Boolean Show(params String[] args) {

            bool displayHelp = false;
            bool displayPort = false;
            bool displayServer = false;
            bool displayRequestor = false;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            options.Add("port", "show the port number.", (v) => {
                displayPort = true;
            });

            options.Add("server", "show the server name.", (v) => {
                displayServer = true;
            });

            options.Add("requestor", "show the default requestor.", (v) => {
                displayRequestor = true;
            });

            options.Add("all", "show all of  the settings.", (v) => {
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("set", options);

                } else if (displayPort) {

                    System.Console.WriteLine("port: {0}", this.Port);

                } else if (displayServer) {

                    System.Console.WriteLine("server: {0}", this.Server);

                } else if (displayRequestor) {

                    System.Console.WriteLine("requestor: {0}", this.Requestor);

                } else {

                    System.Console.WriteLine("port     : {0}", this.Port);
                    System.Console.WriteLine("server   : {0}", this.Server);
                    System.Console.WriteLine("requestor: {0}", this.Requestor);

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

    }

}
