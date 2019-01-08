using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using XAS.Core;
using XAS.Core.Logging;
using XAS.Network.STOMP;
using XAS.Core.Extensions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

using ServiceSpooler.Configuration.Extensions;

namespace ServiceSpooler.Processors {

    /// <summary>
    /// A class to handle the interaction to a STOMP based message queue server.
    /// </summary>
    /// 
    public class Connector {

        private readonly ILogger log = null;
        private readonly Stomp stomp = null;
        private readonly Client client = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        private Task task = null;
        private bool paused = false;
        private ManualResetEventSlim connectedEvent = null;

        /// <summary>
        /// Get/Set the conection event.
        /// </summary>
        /// 
        public ManualResetEventSlim ConnectionEvent { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Connector(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;

            this.connectedEvent = new ManualResetEventSlim(true);
            this.stomp = new Stomp(config, handler, logFactory);
            this.client = new Client(config, handler, logFactory);

            client.OnStompNoop += OnStompNoop;
            client.OnStompError += OnStompError;
            client.OnStompMessage += OnStompMessage;
            client.OnStompReceipt += OnStompReceipt;
            client.OnStompConnected += OnStompConnected;
            client.OnClientDisconnect += OnDisconnect;

            this.log = logFactory.Create(typeof(Connector));

        }

        /// <summary>
        /// Start processing.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            var key = config.Key;
            var section = config.Section;

            paused = false;

            connectedEvent.Set();
            ConnectionEvent.Set();

            client.Cancellation = new CancellationTokenSource();

            task = new Task(() => Processor(false), client.Cancellation.Token, TaskCreationOptions.LongRunning);
            task.Start();

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop processing.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            paused = true;

            if (client.IsConnectionSuccessful) {

                ConnectionEvent.Reset();

                Task[] tasks = { task };

                client.Send(stomp.Disconnect(level: client.Level.ToString()));
                client.Disconnect();

                connectedEvent.Reset();

                client.Cancellation.Cancel(true);
                Task.WaitAll(tasks);

            }

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

        /// <summary>
        /// Start the STOMP processing.
        /// </summary>
        /// 
        public void Processor(Boolean reconnect) {

            log.Trace("Entering Processor()");

            var key = config.Key;
            var section = config.Section;

            string mqPort = config.GetValue(section.Environment(), key.MQPort());
            string mqServer = config.GetValue(section.Environment(), key.MQServer());

            client.Server = config.GetValue(section.MessageQueue(), key.Server(), mqServer);
            client.Username = config.GetValue(section.MessageQueue(), key.Username(), "guest");
            client.Password = config.GetValue(section.MessageQueue(), key.Password(), "guest");
            client.Heartbeat = config.GetValue(section.MessageQueue(), key.Heartbeat(), "0,0");
            client.Port = config.GetValue(section.MessageQueue(), key.Port(), mqPort).ToInt32();
            client.Level = config.GetValue(section.MessageQueue(), key.Level(), "1.0").ToSingle();
            client.UseSSL = config.GetValue(section.MessageQueue(), key.UseSSL(), "false").ToBoolean();
            client.Keepalive = config.GetValue(section.MessageQueue(), key.KeepAlive(), "true").ToBoolean();

            if (reconnect) {

                client.Reconnect();

            } else {

                client.Connect();

            }

            ConnectionEvent.Reset();
            connectedEvent.Wait(client.Cancellation.Token);

            log.Trace("Leaving Processor()");

        }

        /// <summary>
        /// Send a packet.
        /// </summary>
        /// <param name="packet">A Packet object.</param>
        /// 
        public void SendPacket(Packet packet) {

            log.Trace("Entering SendPacket()");

            byte[] message = Stomp.ConvertToBytes(packet.json, client.Level.ToString());
            Frame frame = stomp.Send(
                destination: packet.queue,
                message: message,
                receipt: packet.receipt,
                persistent: true,
                mimeType: "application/json",
                level: client.Level.ToString()
            );

            log.Debug(Utils.Dump(frame));

            client.Send(frame);

            log.Trace("Leaving SendPacket()");

        }

        /// <summary>
        /// Remove a spool file.
        /// </summary>
        /// <param name="filename">The name of the spoole file.</param>
        /// 
        public void UnlinkFile(String filename) {

            log.Trace("Entering UnlinkFile()");

            var key = config.Key;
            var section = config.Section;

            try {

                log.InfoMsg(key.UnlinkFile(), filename);

                File.Delete(filename);

            } catch (Exception ex) {

                log.Warn(ex.Message);

            }

            log.Trace("Leaving UnlinkFile()");

        }

        #region Event Handlers

        /// <summary>
        /// Handle a disconnect event.
        /// </summary>
        /// 
        public void OnDisconnect() {

            log.Trace("Entering OnDisconnect()");

            if (! client.IsConnectionSuccessful && ! paused) {

                connectedEvent.Set();
                ConnectionEvent.Set();

                client.Cancellation = new CancellationTokenSource();

                task = new Task(() => Processor(true), client.Cancellation.Token, TaskCreationOptions.LongRunning);
                task.Start();

            }

            log.Trace("Leaving OnDisconnect()");

        }

        /// <summary>
        /// Handle a STOMP CONNECTED response.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public void OnStompConnected(Frame frame) {

            log.Trace("Entering OnStompConnected()");

            var key = config.Key;
            var section = config.Section;

            log.Debug(Utils.Dump(frame));

            ConnectionEvent.Set();

            log.InfoMsg(key.Connected(), client.Server);

            log.Trace("Leaving OnStompConnected()");

        }

        /// <summary>
        /// Handle a STOMP RECEIPT response.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public void OnStompReceipt(Frame frame) {

            log.Trace("Entering OnStompReceipt()");

            var key = config.Key;
            var section = config.Section;

            log.Debug(String.Format("OnStompReceipt() - frame: {0}", Utils.Dump(frame)));

            if (frame.Headers.ContainsKey("receipt-id")) {

                char[] split = { ';' };  // really, chessy
                string buffer = frame.Headers["receipt-id"];

                log.Debug(String.Format("OnStompReceipt() - receipt: {0}", buffer));
                string receipt = buffer.FromBase64();
                log.Debug(String.Format("OnStompReceipt() - receipt: {0}", receipt));

                if (receipt.Contains(";")) {

                    String[] stuff = receipt.Split(split);

                    if (stuff[1] != "") {

                        UnlinkFile(stuff[1]);

                    }

                } else if (receipt.ToLower() == "disconnected") {

                    log.InfoMsg(key.Disconnected(), client.Server);

                }

            }

            log.Trace("Leaving OnStompReceipt()");

        }

        /// <summary>
        /// Handle a STOMP ERROR response
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public void OnStompError(Frame frame) {

            log.Trace("Entering OnStompError()");

            var key = config.Key;
            var section = config.Section;

            log.Debug(Utils.Dump(frame));

            string body = "";
            string message = "";

            if (frame.Headers.ContainsKey("message")) {

                message = frame.Headers["message"];

            }

            if (frame.Body != null) {

                body = Stomp.ConvertToString(frame.Body, client.Level.ToString());
                body = Regex.Replace(body, @"\r\n?|\n|\r", " ");

            }

            log.ErrorMsg(key.ProtocolError(), message, body);

            log.Trace("Leaving OnStompError()");

        }

        /// <summary>
        /// Handle a STOMP MESSAGE response.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public void OnStompMessage(Frame frame) {

            log.Trace("Entering OnStompMessage()");

            log.Debug(Utils.Dump(frame));

            log.Trace("Leaving OnStompMessage()");

        }

        /// <summary>
        /// Handle a STOMP NOOP response.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public void OnStompNoop(Frame frame) {

            log.Trace("Entering OnStompNoop()");

            log.Debug(Utils.Dump(frame));

            log.Trace("Leaving OnStompNoop()");

        }

        #endregion

    }

}
