using System;
using System.Linq;

using XAS.App;
using XAS.Core.Logging;
using XAS.App.Exceptions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

namespace DemoShell {

    /// <summary>
    /// A command handler class.
    /// </summary>
    /// 
    public class Commands: CommandHandler {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly ILoggerFactory logFactory = null;

        public Int32 Port { get; set; }
        public String Server { get; set; }
        public String Requestor { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguratio object.</param>
        /// <param name="logFactory">An ILoggerFActory object.</param>
        /// 
        public Commands(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory): base() {

            this.config = config;
            this.handler = handler;
            this.logFactory = logFactory;

            this.log = logFactory.Create(typeof(Commands));
            this.Requestor = config.GetValue(config.Section.Environment(), config.Key.Username());

        }

        /// <summary>
        /// Schedule command.
        /// </summary>
        /// <param name="args">An array of args to be used with this command.</param>
        /// <returns>true on success.</returns>
        /// 
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

        /// <summary>
        /// Set command.
        /// </summary>
        /// <param name="args">An array of args to be used with this command.</param>
        /// <returns>true on success.</returns>
        /// 
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

        /// <summary>
        /// Show command.
        /// </summary>
        /// <param name="args">An array of args to be used with this command.</param>
        /// <returns>true on success.</returns>
        /// 
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
