using System;
using System.ServiceProcess;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;
using XAS.App.Configuration.Extensions;

namespace ServiceSupervisor {

    [WindowsService("xas-supervisord",
        DisplayName = "XAS Supervisor",
        Description = "A service that provides supervisor services",
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
        private readonly Processors.Supervisor supervisor = null;

        public Service(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure) {

            this.config = config;
            this.handler = errorHandler;
            this.log = logFactory.Create(typeof(Service));

            var context = Model.Loader.Database(config);
            var repository = new Model.Repositories(config, handler, logFactory, context);
            var manager = new Model.Manager(context, repository);

            this.web = new Processors.Web(config, handler, logFactory, manager);
            this.supervisor = new Processors.Supervisor(config, handler, logFactory, manager);

        }

        public void Dispose() {

        }

        public void OnStart(String[] args) {

            var key = config.Key;

            log.InfoMsg(key.ServiceStartup());

            web.Start();
            supervisor.Start();

        }

        public void OnPause() {

            var key = config.Key;

            log.InfoMsg(key.ServicePaused());

            web.Pause();
            supervisor.Pause();

        }

        public void OnContinue() {

            var key = config.Key;

            log.InfoMsg(key.ServiceResumed());

            web.Continue();
            supervisor.Continue();

        }

        public void OnStop() {

            var key = config.Key;

            log.InfoMsg(key.ServiceStopped());

            web.Stop();
            supervisor.Stop();

        }

        public void OnShutdown() {

            var key = config.Key;

            log.InfoMsg(key.ServiceShutdown());

            web.Shutdown();
            supervisor.Shutdown();

        }

        public void OnCustomCommand(int command) {

            log.Warn(String.Format("Unknown custom command: {0}", command));

        }

    }

}
