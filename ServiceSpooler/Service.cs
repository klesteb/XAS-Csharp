using System;
using System.Threading;
using System.ServiceProcess;
using System.Collections.Concurrent;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;
using XAS.App.Configuration.Extensions;

using ServiceSpooler.Processors;

namespace ServiceSpooler {

    [WindowsService("XasSpoolerd",
        DisplayName = "XAS Spooler",
        Description = "This manages spool files and directories for the XAS environment.",
        EventSource = "XasSpoolerd",
        EventLog = "Application",
        AutoLog = false,
        StartMode = ServiceStartMode.Manual
    )]

    public class Service: IWindowsService {

        private readonly ILogger log = null;
        protected readonly ISecurity security = null;
        protected readonly IConfiguration config = null;
        protected readonly IErrorHandler handler = null;

        private Processors.Monitor monitor = null;
        private Processors.Watchers watchers = null;
        private Processors.Connector connector = null;
 
        private ManualResetEvent dequeueEvent = null;
        private ConcurrentQueue<Packet> queued = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="errorHandler"></param>
        /// <param name="logFactory"></param>
        /// <param name="secure"></param>
        /// 
        public Service(IConfiguration config, IErrorHandler errorHandler, ILoggerFactory logFactory, ISecurity secure) {

            this.config = config;
            this.security = secure;
            this.handler = errorHandler;
            this.log = logFactory.Create(typeof(Service));

            this.queued = new ConcurrentQueue<Packet>();
            this.dequeueEvent = new ManualResetEvent(false);

            this.connector = new Processors.Connector(config, handler, logFactory);
            this.watchers = new Processors.Watchers(config, handler, logFactory, queued);
            this.monitor = new Processors.Monitor(config, handler, logFactory, queued, connector);

        }

        public void Dispose() {

        }

        public void OnStart(String[] args) {

            var key = config.Key;

            log.InfoMsg(key.ServiceStartup());

            watchers.Clear();
            dequeueEvent.Set();

            monitor.DequeueEvent = dequeueEvent;
            watchers.DequeueEvent = dequeueEvent;
            connector.DequeueEvent = dequeueEvent;

            connector.Start();
            watchers.Start();
            monitor.Start();

        }

        public void OnPause() {

            var key = config.Key;

            log.InfoMsg(key.ServicePaused());

            dequeueEvent.Reset();

            watchers.Pause();
            monitor.Pause();
            connector.Pause();

        }

        public void OnContinue() {

            var key = config.Key;

            log.InfoMsg(key.ServiceResumed());

            dequeueEvent.Set();

            watchers.Continue();
            monitor.Continue();
            connector.Continue();

        }

        public void OnStop() {

            var key = config.Key;

            log.InfoMsg(key.ServiceStopped());

            dequeueEvent.Reset();

            watchers.Stop();
            monitor.Stop();
            connector.Stop();

        }

        public void OnShutdown() {

            var key = config.Key;

            log.InfoMsg(key.ServiceShutdown());

            dequeueEvent.Reset();

            watchers.Shutdown();
            monitor.Shutdown();
            connector.Shutdown();

        }

        public void OnCustomCommand(int command) {

            var key = config.Key;

            log.InfoMsg(key.ServiceCustom(), command);

        }

    }

}
