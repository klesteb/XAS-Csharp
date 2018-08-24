using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using XAS.Core.Logging;
using XAS.Core.Extensions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

using ServiceSpooler.Configuration.Extensions;

namespace ServiceSpooler.Processors {

    /// <summary>
    /// A class to setup file system watchers and process any events from them.
    /// </summary>
    /// 
    public class Watchers {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        private List<Task> tasks = null;
        private ConcurrentQueue<Packet> queued = null;
        private Dictionary<string, Watcher> watchers = null;

        /// <summary>
        /// Get/Set AutoResetEvent DequeueEvent.
        /// </summary>
        /// 
        public AutoResetEvent DequeueEvent { get; set; }

        /// <summary>
        /// Get/Set the MnaulResetEvent ConnectionEvent.
        /// </summary>
        /// 
        public ManualResetEvent ConnectionEvent { get; set; }

        /// <summary>
        /// Get/Set the Cancellation Token Source.
        /// </summary>
        public CancellationTokenSource Cancellation { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cfgs">A configuration file.</param>
        /// 
        public Watchers(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ConcurrentQueue<Packet> queued) {

            this.config = config;
            this.queued = queued;
            this.handler = handler;

            this.tasks = new List<Task>();
            this.watchers = new Dictionary<string, Watcher>();
            this.Cancellation = new CancellationTokenSource();
            this.log = logFactory.Create(typeof(Watchers));

        }

        /// <summary>
        /// Clear any queued packets.
        /// </summary>
        /// 
        public void Clear() {

            log.Trace("Entering Clear()");

            if (this.watchers.Count > 0) {

                this.watchers.Clear();

            }

            log.Trace("Leaving Clear()");

        }

        /// <summary>
        /// Add a directory to watch.
        /// </summary>
        /// <param name="directory">A directory.</param>
        /// <param name="watcher">A file system watcher.</param>
        /// 
        public void Add(String directory, Watcher watcher) {

            log.Trace("Entering Add()");

            this.watchers.Add(directory, watcher);

            log.Trace("Leaving Add()");

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

            // build a locker

            var lockName = config.GetValue(section.Application(), key.LockName(), "locked");
            var lockDriver = config.GetValue(section.Application(), key.LockDriver()).ToLockDriver();
            var locker = new XAS.Core.Locking.Factory(lockName).Create(lockDriver);

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

                        watcher.watch = new FileSystemWatcher();
                        watcher.watch.Path = directory;
                        watcher.watch.NotifyFilter = NotifyFilters.LastWrite;
                        watcher.watch.Changed += new FileSystemEventHandler(OnChange);
                        watcher.watch.EnableRaisingEvents = true;

                        this.watchers.Add(directory.TrimIfEndsWith("\\"), watcher);

                        log.InfoMsg(key.WatchDirectory(), directory);

                    } else {

                        log.ErrorMsg(key.NoDirectory(), directory);

                    }

                }

            }

            this.StartEnqueueOrphans();

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop processing.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            foreach (Watcher watcher in this.watchers.Values) {

                watcher.watch.EnableRaisingEvents = false;
                watcher.watch.Dispose();

            }

            this.Cancellation.Cancel(true);
            this.StopEnqueueOrphans();
            this.ConnectionEvent.Reset();

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause processing.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");

            foreach (Watcher watcher in this.watchers.Values) {

                watcher.watch.EnableRaisingEvents = false;

            }

            this.Cancellation.Cancel(true);
            this.StopEnqueueOrphans();
            this.ConnectionEvent.Reset();

            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Continue processing.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue()");

            foreach (Watcher watcher in this.watchers.Values) {

                watcher.watch.EnableRaisingEvents = true;

            }

            this.StartEnqueueOrphans();

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Shutdown the processing.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            foreach (Watcher watcher in this.watchers.Values) {

                watcher.watch.EnableRaisingEvents = false;
                watcher.watch.Dispose();

            }

            this.Cancellation.Cancel(true);
            this.StopEnqueueOrphans();
            this.ConnectionEvent.Reset();

            log.Trace("Leaving Shutdown()");

        }

        /// <summary>
        /// Create and queue a packet.
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

            if (watchers.ContainsKey(directory)) {

                byte[] buffer;
                Watcher watcher = this.watchers[directory];

                if (file.Extension == watcher.spool.Extension) {

                    if ((buffer = watcher.spool.Read(filename)) != null) {

                        Packet packet = new Packet();
                        string rawData = Encoding.UTF8.GetString(buffer);
                        string header = String.Format("{{'hostname':'{0}','timestamp':'{1}','type':'{2}'}}",
                            config.GetValue(section.Environment(), key.Host()),
                            DateTime.Now.ToUnixTime().ToString(),
                            watcher.type
                        );
                          
                        log.Debug(String.Format("EncodePacket() - header: {0}", header));

                        // checking for no data in file.

                        if (! String.IsNullOrEmpty(rawData)) {

                            JObject packetData = JObject.Parse(header);
                            JObject spoolData = JObject.Parse(rawData);

                            packetData.Add("data", spoolData);

                            string jsonData = JsonConvert.SerializeObject(packetData);
                            string receipt = String.Format("{0};{1}", watcher.alias, filename);

                            log.Debug(String.Format("EncodePacket() - jsonData: {0}", jsonData));

                            packet.queue = watcher.queue;
                            packet.json = jsonData;
                            packet.receipt = receipt.ToBase64();

                            this.queued.Enqueue(packet);
                            this.DequeueEvent.Set();

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
        /// Start looking for orphan files.
        /// </summary>
        /// 
        public void StartEnqueueOrphans() {

            log.Trace("Entering StartEnqueueOrphans()");

            foreach (Watcher watcher in watchers.Values) {

                Task task = new Task(() => this.EnqueueOrphans(watcher), this.Cancellation.Token, TaskCreationOptions.LongRunning);
                task.Start();
                tasks.Add(task);

            }

            log.Trace("Leaving StopEnqueueOrphans()");

        }

        /// <summary>
        /// Stop looking for orphan files.
        /// </summary>
        /// 
        public void StopEnqueueOrphans() {

            log.Trace("Entering StopEnqueueOrphans()");

            Task.WaitAny(tasks.ToArray());

            log.Trace("Leaving StopEnqueueOrphans()");

        }

        /// <summary>
        /// Enque any found orphan files.
        /// </summary>
        /// <param name="watcher">A file system watcher.</param>
        /// 
        public void EnqueueOrphans(Watcher watcher) {

            log.Trace("Entering EnqueueOrphans()");
            log.Debug(String.Format("EnqueueOrphans() - processing {0}", watcher.directory));

            this.ConnectionEvent.WaitOne();

            var files = watcher.spool.Scan();

            log.Debug(String.Format("EnqueueOrphans() - found {0} files in {1}", files.Count(), watcher.directory));

            foreach (string file in files) {

                if (this.Cancellation.Token.IsCancellationRequested) {

                    log.Debug("EnqueueOrphans() - cancellation requested");
                    break;

                }

                this.EnqueuePacket(file);

            }

            log.Trace("Leaving EnqueueOrphans()");

        }

        /// <summary>
        /// An fired when an file is created in a watched diretory.
        /// </summary>
        /// <param name="sender">A sender object.</param>
        /// <param name="e">A FileSystemEvent object.</param>
        /// 
        public void OnChange(object sender, FileSystemEventArgs e) {

            log.Trace("Entering OnChange()");

            EnqueuePacket(e.FullPath);

            log.Trace("Leaving OnChange()");

        }

    }

}
