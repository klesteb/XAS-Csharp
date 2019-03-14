using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace ServiceSpooler.Processors {

    /// <summary>
    /// A class to monitor and process any orphaned spool files.
    /// </summary>
    /// 
    public class Monitor {

        private List<Task> tasks = null;
        private Int32 monitorInterval = 300;
        private System.Timers.Timer monitorTimer = null;
        private CancellationTokenSource cancellation = null;

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        /// <summary>
        /// Get/Set the connection event.
        /// </summary>
        /// 
        public ManualResetEventSlim ConnectionEvent { get; set; }

        /// <summary>
        /// Get/Set the directory watchers.
        /// </summary>
        /// 
        public Dictionary<String, Watcher> DirectoryWatchers { get; set; }

        /// <summary>
        /// Set the event handler for enqueueing packets.
        /// </summary>
        /// 
        public event EnqueueHandler OnEnqueuePacket;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="LogFactory">An ILoggerFactory object.</param>
        /// 
        public Monitor(IConfiguration config, IErrorHandler handler, ILoggerFactory LogFactory) {

            this.config = config;
            this.handler = handler;

            this.tasks = new List<Task>();
            this.log = LogFactory.Create(typeof(Monitor));

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

            ConnectionEvent.Wait(cancellation.Token);

            if (! cancellation.IsCancellationRequested) {

                monitorTimer = new System.Timers.Timer(monitorInterval * 1000);
                monitorTimer.Elapsed += ProcessOrphans;

                ProcessOrphans(null, null);

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

            monitorTimer.Stop();
            monitorTimer.Dispose();

            Task.WaitAll(tasks.ToArray());

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause processing.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");

            Stop();

            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Continue processing.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue()");

            Start();

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Shutdown processing.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            Stop();

            log.Trace("Leaving Shutdown()");

        }

        #region Private Methods

        private void ProcessOrphans(object sender, EventArgs args) {

            log.Trace("Entering ProcessOrphans()");

            try {

                monitorTimer.Stop();
                ConnectionEvent.Wait(cancellation.Token);

                foreach (Watcher watcher in DirectoryWatchers.Values) {

                    Task task = new Task(() => EnqueueOrphans(watcher), cancellation.Token, TaskCreationOptions.LongRunning);
                    task.Start();
                    tasks.Add(task);

                    Task.WaitAll(tasks.ToArray());

                }

                tasks.Clear();

                monitorTimer.Start();

            } catch (OperationCanceledException) {

                log.Debug("ProcessOrphans() - Ignored an OperationCanceledException");

            } catch (Exception ex) {

                log.Debug(String.Format("ProcessOrphans() - Ignored an Exception: {0}", ex));

            }

            log.Trace("Leaving ProcessOrphans()");

        }

        /// <summary>
        /// Enqueue any found orphan files.
        /// </summary>
        /// <param name="watcher">A file system watcher.</param>
        /// 
        private void EnqueueOrphans(Watcher watcher) {

            log.Trace("Entering EnqueueOrphans()");
            log.Debug(String.Format("EnqueueOrphans() - processing {0}", watcher.directory));

            try {

                var files = watcher.spool.Scan();

                log.Debug(String.Format("EnqueueOrphans() - found {0} files in {1}", files.Count(), watcher.directory));

                foreach (string file in files) {

                    OnEnqueuePacket(file);

                }

            } catch (OperationCanceledException) {

                log.Debug("EnqueueOrphans() - Ignored an OperationCanceledException");

            } catch (Exception ex) {

                log.Debug(String.Format("EnqueueOrphans() - Ignored an Exception: {0}", ex));

            }

            log.Trace("Leaving EnqueueOrphans()");

        }

    }

    #endregion

}
