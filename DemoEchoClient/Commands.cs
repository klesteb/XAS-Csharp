using System;
using System.Linq;

using XAS.App;
using XAS.Network.TCP;
using XAS.Core.Logging;
using XAS.App.Exceptions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.Network.Configuration.Extensions;

namespace DemoEchoClient {

    /// <summary>
    /// A command handler class.
    /// </summary>
    /// 
    public class Commands: CommandHandler {

        private readonly ILogger log = null;
        private readonly Client client = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;
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
        public Commands(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, Client client): base() {

            this.config = config;
            this.client = client;
            this.handler = handler;
            this.logFactory = logFactory;

            this.log = logFactory.Create(typeof(Commands));
            this.Requestor = config.GetValue(config.Section.Environment(), config.Key.Username());

            client.OnDataReceived += delegate(Byte[] bytes) {
                string buffer = System.Text.Encoding.UTF8.GetString(bytes);
                System.Console.WriteLine("\nreceived: {0}", buffer);
            };

            client.OnException += delegate(Exception ex) {

                System.Console.WriteLine(ex);

            };

            client.OnDisconnect += delegate() {

                var key = config.Key;
                var section = config.Section;
                string format = config.GetValue(section.Messages(), key.ServerDisconnect());

                System.Console.WriteLine(String.Format(format, client.Server));

            };

            client.OnConnect += delegate() {

                var key = config.Key;
                var section = config.Section;
                string format = config.GetValue(section.Messages(), key.ServerConnect());

                System.Console.WriteLine(String.Format(format, client.Server, client.Port));

            };

            client.Connect();

        }

        /// <summary>
        /// Send command.
        /// </summary>
        /// <param name="args">An array of args to be used with this command.</param>
        /// <returns>true on success.</returns>
        /// 
        public Boolean Send(params String[] args) {

            string text = "";
            bool sendText = false;
            bool displayHelp = false;
            string requestor = this.Requestor;

            Options options = new Options(this.config);

            options.Add("help", "outputs a simple help message.", (v) => {
                displayHelp = true;
            });

            options.Add("text=", "send a line of text.", (v) => {
                sendText = true;
                text = v;
            });

            var parameters = options.Parse(args).ToArray(); // forces the options to be parsed

            if (displayHelp) {

                DisplayHelp("send \"<text...>\"", options);

            } else if (sendText) {

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
                client.Send(bytes);

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
                client.Disconnect();
                client.Port = Port;
                client.Reconnect();
            });

            options.Add("server=", "set the server name.", (v) => {
                this.Server = v;
                client.Disconnect();
                client.Server = Server;
                client.Reconnect();
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
