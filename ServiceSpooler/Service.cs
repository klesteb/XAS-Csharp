using System;
using System.IO;
using System.Threading;
using System.ServiceProcess;
using System.Collections.Generic;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;
using XAS.App.Configuration.Extensions;
using XAS.Core.Configuration.Extensions;

using ServiceSpooler.Processors;
using ServiceSpooler.Configuration.Extensions;

namespace ServiceSpooler {

    [WindowsService("xas-spoolerd",
        DisplayName = "XAS Spooler",
        Description = "This manages spool files and directories for the XAS environment.",
        EventSource = "XasSpoolerd",
        EventLog = "Application",
        AutoLog = false,
        StartMode = ServiceStartMode.Manual
    )]

    public class Service: IWindowsService {

        protected readonly ISecurity security = null;
        protected readonly IConfiguration config = null;
        protected readonly IErrorHandler handler = null;

        private readonly ILogger log = null;
        private readonly Processors.Monitor monitor = null;
        private readonly Processors.Watchers watchers = null;
        private readonly Processors.Connector connector = null;
        private readonly Processors.PacketHandler packetHandler = null;

        private ManualResetEventSlim connectionEvent = null;

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

            this.monitor = new Processors.Monitor(config, handler, logFactory);
            this.watchers = new Processors.Watchers(config, handler, logFactory);
            this.connector = new Processors.Connector(config, handler, logFactory);
            this.packetHandler = new Processors.PacketHandler(config, handler, logFactory);

        }

        public void Dispose() {

        }

        public void OnStart(String[] args) {

            var key = config.Key;

            log.InfoMsg(key.ServiceStartup());

            var dirWatchers = BuildWatchers();
            connectionEvent = new ManualResetEventSlim(false);

            monitor.ConnectionEvent = connectionEvent;
            watchers.ConnectionEvent = connectionEvent;
            connector.ConnectionEvent = connectionEvent;
            packetHandler.ConnectionEvent = connectionEvent;

            monitor.DirectoryWatchers = dirWatchers;
            watchers.DirectoryWatchers = dirWatchers;
            packetHandler.DirectoryWatchers = dirWatchers;

            packetHandler.OnDequeuePacket += connector.SendPacket;
            watchers.OnEnqueuePacket += packetHandler.EnqueuePacket;
            monitor.OnEnqueuePacket += packetHandler.EnqueuePacket;

            connector.Start();
            watchers.Start();
            monitor.Start();
            packetHandler.Start();

        }

        public void OnPause() {

            var key = config.Key;

            log.InfoMsg(key.ServicePaused());

            watchers.Pause();
            monitor.Pause();
            connector.Pause();
            packetHandler.Pause();

        }

        public void OnContinue() {

            var key = config.Key;

            log.InfoMsg(key.ServiceResumed());

            var dirWatchers = BuildWatchers();
            connectionEvent = new ManualResetEventSlim(false);

            monitor.ConnectionEvent = connectionEvent;
            watchers.ConnectionEvent = connectionEvent;
            connector.ConnectionEvent = connectionEvent;
            packetHandler.ConnectionEvent = connectionEvent;

            monitor.DirectoryWatchers = dirWatchers;
            watchers.DirectoryWatchers = dirWatchers;
            packetHandler.DirectoryWatchers = dirWatchers;

            watchers.Continue();
            monitor.Continue();
            connector.Continue();
            packetHandler.Continue();

        }

        public void OnStop() {

            var key = config.Key;

            log.InfoMsg(key.ServiceStopped());

            watchers.Stop();
            monitor.Stop();
            connector.Stop();
            packetHandler.Stop();

        }

        public void OnShutdown() {

            var key = config.Key;

            log.InfoMsg(key.ServiceShutdown());

            watchers.Shutdown();
            monitor.Shutdown();
            connector.Shutdown();
            packetHandler.Shutdown();

        }

        public void OnCustomCommand(int command) {

            var key = config.Key;

            log.InfoMsg(key.ServiceCustom(), command);

        }


        private Dictionary<String, Watcher> BuildWatchers() {

            var key = config.Key;
            var section = config.Section;
            var sections = config.GetSections();
            var watchers = new Dictionary<String, Watcher>();

            // build a locker

            var lockName = config.GetValue(section.Application(), key.LockName(), "locked");
            var lockDriver = config.GetValue(section.Application(), key.LockDriver()).ToLockDriver();
            var locker = new XAS.Core.Locking.Factory(lockName).Create(lockDriver);

            // start the watchers

            watchers.Clear();

            foreach (string directory in sections) {

                if ((directory != section.Application()) &&
                    (directory != section.MessageQueue()) &&
                    (directory != section.Environment()) &&
                    (directory != section.Messages())) {

                    if (Directory.Exists(directory)) {

                        Watcher watcher = new Watcher();

                        watcher.queue = config.GetValue(directory, key.Queue(), "");
                        watcher.type = config.GetValue(directory, key.PacketType(), "");
                        watcher.alias = config.GetValue(directory, key.Alias(), "unlink");
                        watcher.directory = directory;

                        watcher.spool = new XAS.Core.Spooling.Spooler(config, locker);
                        watcher.spool.Directory = directory;

                        watchers.Add(directory.TrimIfEndsWith("\\"), watcher);

                        log.InfoMsg(key.WatchDirectory(), directory);

                    } else {

                        log.ErrorMsg(key.NoDirectory(), directory);

                    }

                }

            }

            return watchers;

        }

    }

}
