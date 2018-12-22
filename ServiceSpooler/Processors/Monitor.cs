using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace ServiceSpooler.Processors {

    /// <summary>
    /// A class to monitor and process the queued packets,
    /// </summary>
    /// 
    public class Monitor {

        private Task dequeueTask = null;
        private Connector connector = null;
        private ConcurrentQueue<Packet> queued = null;

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        /// <summary>
        /// Get/Set the DequeuEvent.
        /// </summary>
        /// 
        public AutoResetEvent DequeueEvent { get; set; }

        /// <summary>
        /// Get/Set the MnaulResetEvent ConnectionEvent.
        /// </summary>
        /// 
        public ManualResetEvent ConnectionEvent { get; set; }

        /// <summary>
        /// Get/Set the cancellation token soure.
        /// </summary>
        /// 
        public CancellationTokenSource Cancellation { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Monitor(IConfiguration config, IErrorHandler handler, ILoggerFactory LogFactory, ConcurrentQueue<Packet> queued, Connector connector) {

            this.queued = queued;
            this.connector = connector;
            this.config = config;
            this.handler = handler;

            this.log = LogFactory.Create(typeof(Monitor));

        }

        /// <summary>
        /// Start processing.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            this.dequeueTask = new Task(this.DequeuePacket, this.Cancellation.Token, TaskCreationOptions.LongRunning);
            this.dequeueTask.Start();

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop processing.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            if (this.dequeueTask != null) {

                Task[] tasks = { this.dequeueTask };

                this.Cancellation.Cancel(true);
                this.DequeueEvent.Set();
                Task.WaitAny(tasks);

            }

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause processing.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");

            if (this.dequeueTask != null) {

                Task[] tasks = { this.dequeueTask };

                this.Cancellation.Cancel(true);
                this.DequeueEvent.Set();
                Task.WaitAny(tasks);

            }

            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Continue processing.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue()");

            this.dequeueTask = new Task(this.DequeuePacket, this.Cancellation.Token, TaskCreationOptions.LongRunning);
            this.dequeueTask.Start();

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Shutdown processing.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            if (this.dequeueTask != null) {

                Task[] tasks = { this.dequeueTask };

                this.Cancellation.Cancel(true);
                this.DequeueEvent.Set();
                Task.WaitAny(tasks);

            }

            log.Trace("Leaving Shutdown()");

        }

        /// <summary>
        /// Remove and send a queued packet from the queue.
        /// </summary>
        /// 
        public void DequeuePacket() {

            log.Trace("Entering DequeuePacket()");

            this.ConnectionEvent.WaitOne();
            log.Debug("DequeuePacket() - connected, ready to process");

            for (;;) {

                this.DequeueEvent.WaitOne();
                log.Debug("DequeuePacket() - processing");

                if (this.Cancellation.IsCancellationRequested) {

                    log.Debug("DequeuePacket() - cancellation requested");
                    break;

                }

                Packet packet;

                while (this.queued.TryDequeue(out packet)) {

                    if (this.Cancellation.Token.IsCancellationRequested) {

                        log.Debug("DequeuePacket() - cancellation requested");
                        goto fini;

                    }

                    this.connector.SendPacket(packet);

                }

            }

            fini:
            log.Trace("Leaving DequeuePacket()");

        }

    }

}
