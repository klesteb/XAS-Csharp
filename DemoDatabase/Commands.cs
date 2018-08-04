using System;
using System.Linq;

using XAS.App;
using XAS.Model;
using XAS.Core.Logging;
using XAS.App.Exceptions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModelCommon.DataStructures;

namespace DemoDatabase {

    /// <summary>
    /// Command Handler.
    /// </summary>
    /// 
    public class Commands: CommandHandler {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly ILoggerFactory logFactory = null;
        private readonly DemoModel.Service.Dinosaur dino = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="logFactory">An ILogggerFactory object.</param>
        /// 
        public Commands(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IManager manager): base() {

            this.config = config;
            this.logFactory = logFactory;

            this.log = logFactory.Create(typeof(Commands));
            this.dino = new DemoModel.Service.Dinosaur(config, handler, logFactory, manager);

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

            bool dinoShow = false;
            bool dinoList = false;
            bool displayHelp = false;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            options.Add("dino=", "show a dinosaur", (v) => {
                dinoShow = true;
            });

            options.Add("dinos", "list dinosaurs", (v) => {
                dinoList = true;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("show", options);

                } else if (dinoShow) {

                    if (parameters.Count() > 0) {

                        int id = Convert.ToInt32(parameters[0]);
                        var dto = dino.Get(id);

                        if (dto != null) {

                            System.Console.WriteLine("Id    : {0}", dto.Id);
                            System.Console.WriteLine("Name  : {0}", dto.Name);
                            System.Console.WriteLine("Status: {0}", dto.Status);
                            System.Console.WriteLine("Height: {0}", dto.Height);
                            System.Console.WriteLine("");

                        } else {

                            log.Error(String.Format("Unable to find {0}", id));

                        }

                    } else {

                        log.Error("Invalid parameters, please use \"show -help\" for correct usage");

                    }

                } else if (dinoList) {

                    var dtos = dino.List();
                    string padding = "                               ";

                    System.Console.WriteLine("  Id         Name            Status       Height");
                    System.Console.WriteLine("------+----------------+----------------+--------+");

                    foreach (var dto in dtos) {

                        string name = dto.Name + padding;
                        string status = dto.Status + padding;

                        string output = String.Format(
                            "  {0}      {1}      {2}      {3}",
                            dto.Id, 
                            name.Substring(0, 16), 
                            status.Substring(0, 16),
                            dto.Height
                        );

                        System.Console.WriteLine(output);
                      
                    }

                    System.Console.WriteLine("");

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

        public Boolean Add(params String[] args) {

            string name = "";
            string status = "";
            string height = "0";

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
                height = v;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayAddHelp(options);

                } else {

                    var dti = new DinosaurDTI {
                        Name = name,
                        Status = status,
                        Height = height
                    };

                    int id = dino.Create(dti);
                    if (id != 0) {

                        System.Console.WriteLine("Created: {0}", id);

                    } else {

                        log.Error("Unable to create a new dinosaur");

                    }

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

                    DisplayRemoveHelp(options);

                } else {

                    if (parameters.Count() > 0) {

                        int id = Convert.ToInt32(parameters[0]);

                        if (dino.Delete(id)) {

                            System.Console.WriteLine("Removed: {0}", id);

                        } else {

                            log.Error(String.Format("{0} was not removed", id));

                        }

                    } else {

                        log.Error("Invalid parameters, please use \"remove -help\" for correct usage");

                    }

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

        public Boolean Update(params String[] args) {

            string name = "";
            string status = "";
            string height = "0";

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
                height = v;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayUpdateHelp(options);

                } else {

                    if (parameters.Count() > 0) {

                        int id = Convert.ToInt32(parameters[0]);

                        var dti = new DinosaurDTI {
                            Name = name,
                            Status = status,
                            Height = height
                        };

                        if (dino.Update(id, dti)) {

                            System.Console.WriteLine("Updated: {0}", id);

                        } else {

                            log.Error(String.Format("Unable to update {0}", id));

                        }

                    } else {

                        log.Error("Invalid parameters, please use \"update -help\" for correct usage");

                    }

                }

            } catch (InvalidOptionsException ex) {

                log.Error(ex.Message);

            }

            return true;

        }

        #region Private Methods

        private void DisplayAddHelp(Options options) {

            DisplayHelp("add", options);

            System.Console.WriteLine("Basic command usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To display a single record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id]");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To list the existing records, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute -list");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To add a new record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute -create -category <category> -name <name> -type <type> -value <value>");
            System.Console.WriteLine("");
            System.Console.WriteLine("  This will display the id of the new record.");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To update an existing record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id] -update -category <category> -name <name> -type <type> -value <value>");
            System.Console.WriteLine("");
            System.Console.WriteLine("  This will display the id of the updated record. All options are optional.");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To delete an existing record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id] -delete");
            System.Console.WriteLine("");

        }

        private void DisplayRemoveHelp(Options options) {

            DisplayHelp("remove", options);

            System.Console.WriteLine("Basic command usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To display a single record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id]");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To list the existing records, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute -list");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To add a new record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute -create -category <category> -name <name> -type <type> -value <value>");
            System.Console.WriteLine("");
            System.Console.WriteLine("  This will display the id of the new record.");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To update an existing record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id] -update -category <category> -name <name> -type <type> -value <value>");
            System.Console.WriteLine("");
            System.Console.WriteLine("  This will display the id of the updated record. All options are optional.");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To delete an existing record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id] -delete");
            System.Console.WriteLine("");

        }

        private void DisplayUpdateHelp(Options options) {

            DisplayHelp("update", options);

            System.Console.WriteLine("Basic command usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To display a single record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id]");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To list the existing records, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute -list");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To add a new record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute -create -category <category> -name <name> -type <type> -value <value>");
            System.Console.WriteLine("");
            System.Console.WriteLine("  This will display the id of the new record.");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To update an existing record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id] -update -category <category> -name <name> -type <type> -value <value>");
            System.Console.WriteLine("");
            System.Console.WriteLine("  This will display the id of the updated record. All options are optional.");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To delete an existing record, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    attribute [id] -delete");
            System.Console.WriteLine("");

        }

        #endregion

    }

}
