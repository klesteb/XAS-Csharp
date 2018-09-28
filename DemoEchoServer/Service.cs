using System;
using System.ServiceProcess;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;

using DemoEchoServer.Processors;

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

        private readonly Echo echo = null;
        private readonly ILogger log = null;
        private readonly ISecurity security = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        public Service(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity secure) {

            this.config = config;
            this.security = secure;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Service));

            this.echo = new Echo(config, handler, logFactory);

        }

        public void Dispose() {

        }

        public void OnStart(String[] args) {

            log.InfoMsg("ServiceStartup");

            echo.Start();

        }

        public void OnPause() {

            log.InfoMsg("ServicePaused");

            echo.Pause();

        }

        public void OnContinue() {

            log.InfoMsg("ServiceResumed");

            echo.Resume();

        }

        public void OnStop() {

            log.InfoMsg("ServiceStopped");

            echo.Stop();

        }

        public void OnShutdown() {

            log.InfoMsg("ServiceShutdown");

            echo.Shutdown();

        }

        public void OnCustomCommand(int command) {

            log.Info(String.Format("customcommand: {0}", command));

        }

    }

}
