using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace ServiceSpooler.Processors {

    /// <summary>
    /// A class to setup file system watchers and process any events from them.
    /// </summary>
    /// 
    public class Watchers {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        private CancellationTokenSource cancellation = null;
        private Dictionary<String, Watcher> localWatchers = null;

        /// <summary>
        /// Get/Set the conection event.
        /// </summary>
        /// 
        public ManualResetEventSlim ConnectionEvent { get; set; }

        /// <summary>
        /// Get/Set the directory watchers.
        /// </summary>
        /// 
        public Dictionary<String, Watcher> DirectoryWatchers { get; set; }

        /// <summary>
        /// Set the event handler for when enqueueing packets.
        /// </summary>
        /// 
        public event EnqueueHandler OnEnqueuePacket;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Watchers(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;

            this.log = logFactory.Create(typeof(Watchers));
            this.localWatchers = new Dictionary<String, Watcher>();

        }

        /// <summary>
        /// Start processing.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            var key = config.Key;
            var section = config.Section;
            var sections = config.GetSections();

            cancellation = new CancellationTokenSource();

            // block until connected

            ConnectionEvent.Wait(cancellation.Token);

            if (! cancellation.IsCancellationRequested) {

                foreach (KeyValuePair<String, Watcher> item in DirectoryWatchers) {

                    var watcher = new Watcher();

                    watcher.queue = item.Value.queue;
                    watcher.type = item.Value.type;
                    watcher.alias = item.Value.alias;
                    watcher.directory = item.Value.directory;

                    watcher.spool = item.Value.spool;
                    watcher.spool.Directory = item.Value.directory;

                    watcher.watch = new FileSystemWatcher();
                    watcher.watch.Path = watcher.directory;
                    watcher.watch.NotifyFilter = NotifyFilters.LastWrite;
                    watcher.watch.Changed += new FileSystemEventHandler(OnChange);
                    watcher.watch.EnableRaisingEvents = true;

                    localWatchers.Add(item.Key, watcher);

                }

            }

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop processing.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            cancellation.Cancel(true);

            foreach (Watcher watcher in localWatchers.Values) {

                watcher.watch.EnableRaisingEvents = false;
                watcher.watch.Dispose();

            }

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause processing.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");

            cancellation.Cancel(true);

            foreach (Watcher watcher in localWatchers.Values) {

                watcher.watch.EnableRaisingEvents = false;

            }

            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Continue processing.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue()");

            cancellation = new CancellationTokenSource();

            foreach (Watcher watcher in localWatchers.Values) {

                watcher.watch.EnableRaisingEvents = true;

            }

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Shutdown the processing.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            Stop();

            log.Trace("Leaving Shutdown()");

        }

        /// <summary>
        /// Fired when an file is created in a watched diretory.
        /// </summary>
        /// <param name="sender">A sender object.</param>
        /// <param name="e">A FileSystemEvent object.</param>
        /// 
        public void OnChange(object sender, FileSystemEventArgs e) {

            log.Trace("Entering OnChange()");

            OnEnqueuePacket(e.FullPath);

            log.Trace("Leaving OnChange()");

        }

    }

}
