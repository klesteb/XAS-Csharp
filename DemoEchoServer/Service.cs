using System;
using System.Text;
using System.ServiceProcess;

using XAS.Network.TCP;
using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Extensions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;

using DemoEchoServer.Configuration.Extensions;

namespace DemoEchoServer {

    [WindowsService("DemoEchoServer",
        DisplayName = "DemoEchoServer",
        Description = "A basic echo server",
        EventSource = "DemoEchoServer",
        EventLog = "Application",
        AutoLog = true,
        StartMode = ServiceStartMode.Manual
    )]

    public class Service: IWindowsService {

        private readonly ILogger log = null;
        private readonly Server server = null;
        private readonly Decoder decoder = null;
        private readonly ISecurity security = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        public Service(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity secure) {

            this.config = config;
            this.security = secure;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Service));

            var key = config.Key;
            var section = config.Section;

            this.decoder = Encoding.UTF8.GetDecoder();
            this.server = new Server(config, handler, logFactory);

            this.server.Port = config.GetValue(section.Server(), key.Port(), "7").ToInt32();
            this.server.Address = config.GetValue(section.Server(), key.Address(), "127.0.0.1");
            this.server.Backlog = config.GetValue(section.Server(), key.Backlog(), "10").ToInt32();
            this.server.ClientTimeout = config.GetValue(section.Server(), key.ClientTimeout(), "0").ToInt32();
            this.server.ReaperInterval = config.GetValue(section.Server(), key.ReaperInterval(), "60").ToInt32();

            this.server.SSLCaCert = config.GetValue(section.SSL(), key.SSLCaCert());
            this.server.UseSSL = config.GetValue(section.SSL(), key.UseSSL(), "false").ToBoolean();
            this.server.SSLVerifyPeer = config.GetValue(section.SSL(), key.SSLVerifyPeer(), "false").ToBoolean();

            this.server.OnDataSent += OnDataSent;
            this.server.OnDataReceived += OnDataReceived;

        }

        public void Dispose() {

        }

        public void OnDataReceived(Int32 id, byte[] buffer) {

            int bytes = buffer.Length;
            var client = server.GetClient(id);
            StringBuilder message = new StringBuilder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
            decoder.GetChars(buffer, 0, bytes, chars, 0);
            message.Append(chars);

            log.Info(String.Format("Received data from: {0}, on port: {1}, data: {2}", 
                client.RemoteHost, client.RemotePort, message.ToString()));

            server.Send(id, buffer);

        }

        public void OnDataSent(Int32 id) {

            var client = server.GetClient(id);

            log.Info(String.Format("Sent data to: {0} of port {1}", client.RemoteHost, client.RemotePort));

        }
        
        public void OnStart(String[] args) {

            log.InfoMsg("ServiceStartup");
            server.Start();

        }

        public void OnPause() {

            log.InfoMsg("ServicePaused");
            server.Pause();

        }

        public void OnContinue() {

            log.InfoMsg("ServiceResumed");
            server.Resume();

        }

        public void OnStop() {

            log.InfoMsg("ServiceStopped");
            server.Stop();

        }

        public void OnShutdown() {

            log.InfoMsg("ServiceShutdown");
            server.Shutdown();

        }

        public void OnCustomCommand(int command) {

            log.Info(String.Format("customcommand: {0}", command));

        }

    }

}
