using System;
using System.ServiceProcess;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;

namespace ServiceSupervisor {

    [WindowsService("xas-supervisord",
        DisplayName = "XAS Supervisor",
        Description = "A service to provide supervisor services",
        EventSource = "XasSupervisord",
        EventLog = "Application",
        AutoLog = false,
        StartMode = ServiceStartMode.Manual
    )]

    public class Service: IWindowsService {

        private readonly ILogger log = null;
        private readonly Processors.Web web = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        public Service(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure) {

            this.config = config;
            this.handler = errorHandler;
            this.log = logFactory.Create(typeof(Service));

            this.web = new Processors.Web(config, handler, logFactory);

        }

        public void Dispose() {

        }

        public void OnStart(String[] args) {

            log.InfoMsg("ServiceStartup");

            web.Start();

        }

        public void OnPause() {

            log.InfoMsg("ServicePaused");

            web.Pause();

        }

        public void OnContinue() {

            log.InfoMsg("ServiceResumed");

            web.Continue();

        }

        public void OnStop() {

            log.InfoMsg("ServiceStopped");

            web.Stop();

        }

        public void OnShutdown() {

            log.InfoMsg("ServiceShutdown");

            web.Shutdown();

        }

        public void OnCustomCommand(int command) {

            log.Warn(String.Format("Unknown custom command: {0}", command));

        }

    }

}
