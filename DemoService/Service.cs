using System;
using System.ServiceProcess;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;

namespace DemoService {

    [WindowsService("DemoService",
        DisplayName = "DemoService",
        Description = "The description of the DemoService service.",
        EventSource = "DemoService",
        EventLog = "Application",
        AutoLog = true,
        StartMode = ServiceStartMode.Manual
    )]

    public class Service: IWindowsService {

        private readonly ILogger log = null;
        protected readonly ISecurity security = null;
        protected readonly IConfiguration config = null;
        protected readonly IErrorHandler handler = null;

        public Service(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure) {

            this.config = config;
            this.security = secure;
            this.handler = errorHandler;
            this.log = logFactory.Create(typeof(Service));

        }

        public void Dispose() {


        }

        public void OnStart(String[] args) {

            log.InfoMsg("ServiceStartup");

        }

        public void OnPause() {

            log.InfoMsg("ServicePaused");

        }

        public void OnContinue() {

            log.InfoMsg("ServiceResumed");

        }

        public void OnStop() {

            log.InfoMsg("ServiceStopped");

        }

        public void OnShutdown() {

            log.InfoMsg("ServiceShutdown");

        }

        public void OnCustomCommand(int command) {

            log.Info(String.Format("customcommand: {0}", command));

        }

    }

}

