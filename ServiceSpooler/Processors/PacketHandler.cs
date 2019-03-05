using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

using ServiceSpooler.Configuration.Extensions;

namespace ServiceSpooler.Processors {
    
    /// <summary>
    /// A class to maintain the enqueueing and dequeueing of packets
    /// </summary>
    /// 
    public class PacketHandler {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly ConcurrentQueue<Packet> queued = null;

        private Task task = null;
        private ManualResetEventSlim dequeueEvent = null;
        private CancellationTokenSource cancellation = null;

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
        /// Set the event handler for dequeueing packets/
        /// </summary>
        /// 
        public event DequeueHandler OnDequeuePacket;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="handler"></param>
        /// <param name="logFactory"></param>        
        /// 
        public PacketHandler(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;

            this.queued = new ConcurrentQueue<Packet>();
            this.dequeueEvent = new ManualResetEventSlim();
            this.log = logFactory.Create(typeof(PacketHandler));

        }

        public void Start() {

            log.Trace("Entering Start()");

            cancellation = new CancellationTokenSource();

            task = new Task(() => DequeuePacket(), cancellation.Token, TaskCreationOptions.LongRunning);
            task.Start();

            log.Trace("Leaving Start()");

        }

        public void Stop() {

            log.Trace("Entering Stop()");

            Task[] tasks = { task };

            cancellation.Cancel(true);
            Task.WaitAll(tasks);

            log.Trace("Leaving Stop()");

        }

        public void Pause() {

            log.Trace("Entering Pause()");

            Stop();

            log.Trace("Leaving Pause()");

        }

        public void Continue() {

            log.Trace("Entering Contunue()");

            Start();

            log.Trace("Leaving Continue()");

        }

        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            Stop();

            log.Trace("Leaving Shutdown()");

        }

        /// <summary>
        /// The event handler for a OnEnqueuePacket event.
        /// </summary>
        /// <param name="filename">The name of the spool file.</param>
        /// 
        public void EnqueuePacket(string filename) {

            log.Trace("Entering EnqueuePacket()");

            var key = config.Key;
            var section = config.Section;
            FileInfo file = new FileInfo(filename);
            string directory = file.DirectoryName;

            log.Debug(String.Format("EnqueuePacket() - file: {0}", file.FullName));

            if (DirectoryWatchers.ContainsKey(directory)) {

                byte[] buffer;
                Watcher watcher = DirectoryWatchers[directory];

                if (file.Extension == watcher.spool.Extension) {

                    if ((buffer = watcher.spool.Read(filename)) != null) {

                        Packet packet = new Packet();
                        string rawData = Encoding.UTF8.GetString(buffer);
                        string header = String.Format(
                            "{{'hostname':'{0}','timestamp':'{1}','type':'{2}'}}",
                            config.GetValue(section.Environment(), key.Host()),
                            DateTime.Now.ToUnixTime().ToString(),
                            watcher.type
                        );

                        log.Debug(String.Format("EncodePacket() - header: {0}", header));

                        // checking for no data in file.

                        if (!String.IsNullOrEmpty(rawData)) {

                            JObject packetData = JObject.Parse(header);
                            JObject spoolData = JObject.Parse(rawData);

                            packetData.Add("data", spoolData);

                            string jsonData = JsonConvert.SerializeObject(packetData);
                            string receipt = String.Format("{0};{1}", watcher.alias, filename);

                            log.Debug(String.Format("EncodePacket() - jsonData: {0}", jsonData));

                            packet.queue = watcher.queue;
                            packet.json = jsonData;
                            packet.receipt = receipt.ToBase64();

                            queued.Enqueue(packet);
                            dequeueEvent.Set();

                            log.InfoMsg(key.FileFound(), filename, watcher.queue);

                        } else {

                            file.Delete();
                            log.WarnMsg(key.NoData(), filename, watcher.queue);

                        }

                    } else {

                        file.Delete();
                        log.WarnMsg(key.CorruptFile(), filename);

                    }

                }

            } else {

                log.WarnMsg(key.UnknownFile(), filename);

            }

            log.Trace("Leaving EnqueuePacket()");

        }

        /// <summary>
        /// The event handler for the OnDequeuePacket event.
        /// </summary>
        /// 
        public void DequeuePacket() {

            log.Trace("Entering DequeuePacket()");
            log.Debug("DequeuePacket() - connected, ready to process");

            try {

                for (;;) {

                    ConnectionEvent.Wait(cancellation.Token);
                    log.Debug("DequeuePacket() - after ConnectionEvent.Wait()");
                    dequeueEvent.Wait(cancellation.Token);

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

                        OnDequeuePacket(packet);

                    }

                    dequeueEvent.Reset();

                }

                fini:;

            } catch (OperationCanceledException) {

                // ignore, the waits were canceled.

                log.Debug("DequeuePacket() - Ignored but a OperationCanceledException was thrown");

            } catch (Exception ex) {

                handler.Exceptions(ex);

            }

            log.Trace("Leaving DequeuePacket()");

        }

    }

}
