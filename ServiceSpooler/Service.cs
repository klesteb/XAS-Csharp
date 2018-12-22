﻿using System;
using System.Threading;
using System.ServiceProcess;
using System.Collections.Concurrent;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;

using ServiceSpooler.Processors;
using ServiceSpooler.Configuration.Extensions;

namespace ServiceSpooler {

    [WindowsService("XasSpooler",
        DisplayName = "XAS Spooler",
        Description = "This manages spool files and directories for the XAS environment.",
        EventSource = "XasSpooler",
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
 
        private AutoResetEvent dequeueEvent = null;
        private ConcurrentQueue<Packet> queued = null;
        private ManualResetEvent connectionEvent = null;
        private CancellationTokenSource cancelToken = null;

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
            this.dequeueEvent = new AutoResetEvent(false);
            this.connectionEvent = new ManualResetEvent(false);

            this.connector = new Processors.Connector(config, handler, logFactory);
            this.watchers = new Processors.Watchers(config, handler, logFactory, queued);
            this.monitor = new Processors.Monitor(config, handler, logFactory, queued, connector);

        }

        public void Dispose() {

        }

        public void OnStart(String[] args) {

            log.InfoMsg("ServiceStartup");

            this.cancelToken = new CancellationTokenSource();

            this.watchers.Clear();

            this.watchers.Cancellation = this.cancelToken;
            this.watchers.DequeueEvent = this.dequeueEvent;
            this.watchers.ConnectionEvent = this.connectionEvent;

            this.connector.Cancellation = this.cancelToken;
            this.connector.ConnectionEvent = this.connectionEvent;

            this.monitor.Cancellation = this.cancelToken;
            this.monitor.DequeueEvent = this.dequeueEvent;
            this.monitor.ConnectionEvent = this.connectionEvent;

            this.connector.Cancellation = this.cancelToken;

            connector.Start();

            if (! this.cancelToken.IsCancellationRequested) {

                watchers.Start();
                monitor.Start();

            }

        }

        public void OnPause() {

            log.InfoMsg("ServicePaused");

            this.cancelToken.Cancel();

            this.watchers.Pause();
            this.monitor.Pause();
            this.connector.Pause();

        }

        public void OnContinue() {

            log.InfoMsg("ServiceResumed");

            this.cancelToken.Dispose();
            this.cancelToken = new CancellationTokenSource();

            this.watchers.Cancellation = this.cancelToken;
            this.watchers.Continue();

            this.monitor.Cancellation = this.cancelToken;
            this.monitor.Continue();

            this.connector.Cancellation = this.cancelToken;
            this.connector.Continue();

        }

        public void OnStop() {

            log.InfoMsg("ServiceStopped");

            this.cancelToken.Cancel();

            this.watchers.Stop();
            this.monitor.Stop();
            this.connector.Stop();

        }

        public void OnShutdown() {

            log.InfoMsg("ServiceShutdown");

            this.cancelToken.Cancel();

            this.watchers.Shutdown();
            this.monitor.Shutdown();
            this.connector.Shutdown();

        }

        public void OnCustomCommand(int command) {

            log.Info(String.Format("customcommand: {0}", command));

        }

    }

}
