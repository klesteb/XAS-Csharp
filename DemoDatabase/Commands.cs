using System;
using System.Linq;

using XAS.App;
using XAS.App.Exceptions;

using XAS.Core.Logging;
using XAS.Core.Configuration;

namespace DemoDatabase {

    /// <summary>
    /// Command Handler.
    /// </summary>
    /// 
    public class Commands: CommandHandler {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly ILoggerFactory logFactory = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="logFactory">An ILogggerFactory object.</param>
        /// 
        public Commands(IConfiguration config, ILoggerFactory logFactory): base() {

            this.config = config;
            this.logFactory = logFactory;

            this.log = logFactory.Create(typeof(Commands));

        }

        /// <summary>
        /// Set various global values.
        /// </summary>
        /// <param name="args">Command args.</param>
        /// <returns>true on success.</returns>
        /// 
        public Boolean Set(params String[] args) {

            bool displayHelp = false;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("set", options);

                } else {

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

        /// <summary>
        /// Show various global values.
        /// </summary>
        /// <param name="args">Command args.</param>
        /// <returns>true on success.</returns>
        /// 
        public Boolean Show(params String[] args) {

            bool displayHelp = false;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("set", options);

                } else {

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

        public Boolean Add(params String[] args) {

            int height = 0;
            string name = "";
            string status = "";

            bool displayHelp = false;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            options.Add("name", "the dinosaurs name", (v) => {
                name = v;
            });

            options.Add("status", "the dinosaurs status", (v) => {
                status = v;
            });

            options.Add("height", "the dinosaurs height", (v) => {
                height = Convert.ToInt32(v); ;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("add", options);

                } else {

                    int id = Convert.ToInt32(parameters[0]);

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

        public Boolean Remove(params String[] args) {

            bool displayHelp = false;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("remove", options);

                } else {

                    int id = Convert.ToInt32(parameters[0]);

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

        public Boolean Update(params String[] args) {

            int height = 0;
            string name = "";
            string status = "";

            bool displayHelp = false;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            options.Add("name", "the dinosaurs name", (v) => {
                name = v;
            });

            options.Add("status", "the dinosaurs status", (v) => {
                status = v;
            });

            options.Add("height", "the dinosaurs height", (v) => {
                height = Convert.ToInt32(v);
                ;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("update", options);

                } else {

                    int id = Convert.ToInt32(parameters[0]);

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

    }

}
