using System;
using System.Linq;

using XAS.App;
using XAS.Core.Logging;
using XAS.App.Exceptions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModelCommon.DataStructures;
using XAS.Core.Configuration.Extensions;
using DemoMicroServiceClient.Configuration.Extensions;

namespace DemoMicroServiceClient {

    /// <summary>
    /// Command Handler.
    /// </summary>
    /// 
    public class Commands: CommandHandler {

        private Int32 port = 8080;
        private string username = "";
        private string password = "";
        private string server = "localost";

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly ILoggerFactory logFactory = null;
        private readonly DemoMicroServiceClient.Client dino = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="logFactory">An ILogggerFactory object.</param>
        /// 
        public Commands(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory): base() {

            this.config = config;
            this.handler = handler;
            this.logFactory = logFactory;

            var key = config.Key;
            var section = config.Section;

            this.username = config.GetValue(section.Environment(), key.Username());
            this.server = config.GetValue(section.Service(), key.Server(), "localhost");
            this.port = Convert.ToInt32(config.GetValue(section.Service(), key.Port(), "8080"));

            this.dino = new Client(config, handler, logFactory);
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

            options.Add("username=", "set a new username.", (v) => {
                this.username = v;
            });

            options.Add("password=", "set a new password.", (v) => {
                this.password = v;
            });

            options.Add("server=", "set a new server to use.", (v) => {
                this.server = v;
            });

            options.Add("port=", "set the port on the server.", (v) => {
                this.port = Convert.ToInt32(v);
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

            Int32 id = 0;
            bool dinoShow = false;
            bool dinoList = false;
            bool displayHelp = false;
            bool displayPort = false;
            bool displayServer = false;
            bool displayUsername = false;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            options.Add("dino=", "get a dinosaur", (v) => {
                id = Convert.ToInt32(v);
                dinoShow = true;
            });

            options.Add("list-dinos", "list dinosaurs", (v) => {
                dinoList = true;
            });

            options.Add("username", "show current username", (v) => {
                displayUsername = true;
            });

            options.Add("port", "show current port", (v) => {
                displayPort = true;
            });

            options.Add("server", "show current server", (v) => {
                displayServer = true;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayHelp("show", options);

                } else if (dinoShow) {

                    dino.Port = port;
                    dino.Server = server;
                    dino.Username = username;
                    dino.Password = password;

                    var dto = dino.Get(id);

                    if (dto != null) {

                        System.Console.WriteLine("");
                        System.Console.WriteLine("Id    : {0}", dto.Id);
                        System.Console.WriteLine("Name  : {0}", dto.Name);
                        System.Console.WriteLine("Status: {0}", dto.Status);
                        System.Console.WriteLine("Height: {0}", dto.Height);
                        System.Console.WriteLine("");

                    } else {

                        log.Error(String.Format("Unable to find {0}", id));

                    }

                } else if (dinoList) {

                    dino.Port = port;
                    dino.Server = server;
                    dino.Username = username;
                    dino.Password = password;

                    var dtos = dino.List();
                    string padding = "                               ";

                    System.Console.WriteLine("");
                    System.Console.WriteLine("  Id          Name             Status        Height");
                    System.Console.WriteLine("-------+-----------------+-----------------+--------+");

                    foreach (var dto in dtos) {

                        string xid = dto.Id.ToString() + padding;
                        string name = dto.Name + padding;
                        string status = dto.Status + padding;

                        string output = String.Format(
                            "  {0}  {1}  {2}  {3}",
                            xid.Substring(0, 5),
                            name.Substring(0, 16),
                            status.Substring(0, 16),
                            dto.Height
                        );

                        System.Console.WriteLine(output);

                    }

                    System.Console.WriteLine("");

                } else if (displayUsername) {

                    System.Console.WriteLine("Username: {0}", username);

                } else if (displayServer) {

                    System.Console.WriteLine("Server: {0}", server);

                } else if (displayPort) {

                    System.Console.WriteLine("Port: {0}", port);

                } else {

                    System.Console.WriteLine("");
                    System.Console.WriteLine("Global Settings");
                    System.Console.WriteLine("------------------------");
                    System.Console.WriteLine("Username: {0}", username);
                    System.Console.WriteLine("Server  : {0}", server);
                    System.Console.WriteLine("Port    : {0}", port);
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

            options.Add("name=", "the dinosaurs name", (v) => {
                name = v;
            });

            options.Add("status=", "the dinosaurs status", (v) => {
                status = v;
            });

            options.Add("height=", "the dinosaurs height", (v) => {
                height = v;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayAddHelp(options);

                } else {

                    var post = new DinosaurPost {
                        Name = name,
                        Status = status,
                        Height = height
                    };

                    dino.Port = port;
                    dino.Server = server;
                    dino.Username = username;
                    dino.Password = password;

                    int id = dino.Create(post);
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

                        dino.Port = port;
                        dino.Server = server;
                        dino.Username = username;
                        dino.Password = password;

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

            options.Add("name=", "the dinosaurs name", (v) => {
                name = v;
            });

            options.Add("status=", "the dinosaurs status", (v) => {
                status = v;
            });

            options.Add("height=", "the dinosaurs height", (v) => {
                height = v;
            });

            try {

                var parameters = options.Parse(args).ToArray();

                if (displayHelp) {

                    DisplayUpdateHelp(options);

                } else {

                    if (parameters.Count() > 0) {

                        dino.Port = port;
                        dino.Server = server;
                        dino.Username = username;
                        dino.Password = password;

                        int id = Convert.ToInt32(parameters[0]);

                        var update = new DinosaurUpdate {
                            Name = name,
                            Status = status,
                            Height = height
                        };

                        if (dino.Update(id, update)) {

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
            System.Console.WriteLine("  To add a new dinosaur do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    add -name <name> -status <status> -height <height>");
            System.Console.WriteLine("");

        }

        private void DisplayRemoveHelp(Options options) {

            DisplayHelp("remove", options);

            System.Console.WriteLine("Basic command usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To remove a dinosaur, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    remove [id]");
            System.Console.WriteLine("");

        }

        private void DisplayUpdateHelp(Options options) {

            DisplayHelp("update", options);

            System.Console.WriteLine("Basic command usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("  To update a dinosaur, do the following:");
            System.Console.WriteLine("");
            System.Console.WriteLine("    update [id] -name <name> -status <status> -height <height>");
            System.Console.WriteLine("");

        }

        #endregion

    }

}
