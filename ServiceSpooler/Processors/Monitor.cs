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
        private CancellationTokenSource cancellation = null;

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        /// <summary>
        /// Get/Set the DequeuEvent.
        /// </summary>
        /// 
        public AutoResetEvent DequeueEvent { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Monitor(IConfiguration config, IErrorHandler handler, ILoggerFactory LogFactory, ConcurrentQueue<Packet> queued, Connector connector) {

            this.queued = queued;
            this.config = config;
            this.handler = handler;
            this.connector = connector;

            this.log = LogFactory.Create(typeof(Monitor));

        }

        /// <summary>
        /// Start processing.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            cancellation = new CancellationTokenSource();

            dequeueTask = new Task(DequeuePacket, cancellation.Token, TaskCreationOptions.LongRunning);
            dequeueTask.Start();

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop processing.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            if (dequeueTask != null) {

                Task[] tasks = { dequeueTask };

                cancellation.Cancel(true);
                DequeueEvent.Set();

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

            if (dequeueTask != null) {

                Task[] tasks = { dequeueTask };

                cancellation.Cancel(true);
                DequeueEvent.Set();
                
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

            cancellation = new CancellationTokenSource();

            dequeueTask = new Task(DequeuePacket, cancellation.Token, TaskCreationOptions.LongRunning);
            dequeueTask.Start();

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Shutdown processing.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            if (dequeueTask != null) {

                Task[] tasks = { dequeueTask };

                cancellation.Cancel(true);
                DequeueEvent.Set();
                
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
            log.Debug("DequeuePacket() - connected, ready to process");

            for (;;) {

                DequeueEvent.WaitOne();

                log.Debug("DequeuePacket() - processing");

                if (cancellation.IsCancellationRequested) {

                    log.Debug("DequeuePacket() - cancellation requested");
                    goto fini;

                }

                Packet packet;

                while (queued.TryDequeue(out packet)) {

                    if (cancellation.IsCancellationRequested) {

                        log.Debug("DequeuePacket() - cancellation requested");
                        goto fini;

                    }

                    connector.SendPacket(packet);

                }

            }

            fini:
            log.Trace("Leaving DequeuePacket()");

        }

    }

}
