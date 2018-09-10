using System;
using System.IO;
using System.Threading;
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
    public class Connector: Client {

        private readonly ILogger log = null;

        /// <summary>
        /// Get/Set the connection event.
        /// </summary>
        /// 
        public ManualResetEvent ConnectionEvent { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cfgs">A configuration object.</param>
        /// <param name="level">The STOMP level to use.</param>
        /// 
        public Connector(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory): 
            base(config, handler, logFactory, "1.0") {

            var key = config.Key;
            var section = config.Section;
            string mqServer = config.GetValue(section.Environment(), key.MQServer());
            string mqPort = config.GetValue(section.Environment(), key.MQPort());

            this.Cancellation = new CancellationTokenSource();
            this.Server = config.GetValue(section.MessageQueue(), key.Server(), mqServer);
            this.Username = config.GetValue(section.MessageQueue(), key.Username(), "guest");
            this.Password = config.GetValue(section.MessageQueue(), key.Password(), "guest");
            this.Level = config.GetValue(section.MessageQueue(), key.Level(), "1.0").ToSingle();
            this.Port = config.GetValue(section.MessageQueue(), key.MQPort(), mqPort).ToInt32();

            this.log = logFactory.Create(typeof(Connector));

        }

        /// <summary>
        /// Send a packet.
        /// </summary>
        /// <param name="packet">a packet object.</param>
        /// 
        public void SendPacket(Packet packet) {

            log.Trace("Entering SendPacket()");

            byte[] message = Stomp.ConvertToBytes(packet.json, this.Level.ToString());
            Frame frame = stomp.Send(
                destination: packet.queue,
                message: message,
                receipt: packet.receipt,
                persistent: true,
                mimeType: "application/json",
                level: this.Level.ToString()
            );

            log.Debug(Utils.Dump(frame));

            this.Send(frame);

            log.Trace("Leaving SendPacket()");

        }

        /// <summary>
        /// Remove a spool file.
        /// </summary>
        /// <param name="filename">The name of the spoole file.</param>
        /// 
        public void UnlinkFile(String filename) {

            var key = config.Key;
            var section = config.Section;

            log.Trace("Entering UnlinkFile()");

            try {

                log.InfoMsg(key.UnlinkFile(), filename);

                File.Delete(filename);

            } catch (Exception ex) {

                log.Warn(ex.Message);

            }

            log.Trace("Leaving UnlinkFile()");

        }

        /// <summary>
        /// Start processing.
        /// </summary>
        /// 
        public void Start() {

            var key = config.Key;
            var section = config.Section;

            log.Trace("Entering Start()");

            string mqPort = config.GetValue(section.Environment(), key.MQPort());
            string mqServer = config.GetValue(section.Environment(), key.MQServer());

            this.Server = config.GetValue(section.MessageQueue(), key.Server(), mqServer);
            this.Username = config.GetValue(section.MessageQueue(), key.Username(), "guest");
            this.Password = config.GetValue(section.MessageQueue(), key.Password(), "guest");
            this.Port = config.GetValue(section.MessageQueue(), key.Port(), mqPort).ToInt32();
            this.Level = config.GetValue(section.MessageQueue(), key.Level(), "1.0").ToSingle();
            this.UseSSL = config.GetValue(section.MessageQueue(), key.UseSSL(), "false").ToBoolean();
            this.Keepalive = config.GetValue(section.MessageQueue(), key.KeepAlive(), "true").ToBoolean();

            this.Connect();

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop processing.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            this.Send(stomp.Disconnect(receipt: "disconnected", level: this.Level.ToString()));
            this.Disconnect();
            this.ConnectionEvent.Reset();

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause processing.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");

            this.Send(stomp.Disconnect(receipt: "disconnected", level: this.Level.ToString()));
            this.Disconnect();
            this.ConnectionEvent.Reset();

            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Continue processing.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue()");

            this.Connect();
            this.ConnectionEvent.Set();

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Shutdown processing.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            this.Send(stomp.Disconnect(receipt: "disconnected", level: this.Level.ToString()));
            this.Disconnect();
            this.ConnectionEvent.Reset();

            log.Trace("Leaving Shutdown()");

        }

        /// <summary>
        /// Handle a STOMP CONNECTED response.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public override void OnConnected(Frame frame) {

            var key = config.Key;
            var section = config.Section;

            log.Trace("Entering OnConnected()");
            log.Debug(Utils.Dump(frame));


            log.InfoMsg(key.Connected(), this.Server, this.Port);
            this.ConnectionEvent.Set();

            log.Trace("Leaving OnConnected()");

        }

        /// <summary>
        /// Handle a STOMP RECEIPT response.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public override void OnReceipt(Frame frame) {

            var key = config.Key;
            var section = config.Section;

            log.Trace("Entering OnReceipt()");
            log.Debug(String.Format("frame: {0}", Utils.Dump(frame)));

            if (frame.Headers.ContainsKey("receipt-id")) {

                char[] split = { ';' };  // really, chessy
                string buffer = frame.Headers["receipt-id"];

                log.Debug(String.Format("OnReceipt() - receipt: {0}", buffer));
                string receipt = buffer.FromBase64();
                log.Debug(String.Format("OnReceipt() - receipt: {0}", receipt));

                if (receipt.Contains(";")) {

                    String[] stuff = receipt.Split(split);

                    if (stuff[1] != "") {

                        this.UnlinkFile(stuff[1]);

                    }

                } else if (receipt.ToLower() == "disconnected") {

                    log.InfoMsg(key.Disconnected(), this.Server);

                }

            }

            log.Trace("Leaving OnReceipt()");

        }

        /// <summary>
        /// Handle a STOMP ERROR response
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public override void OnError(Frame frame) {

            var key = config.Key;
            var section = config.Section;

            log.Trace("Entering OnError()");
            log.Debug(Utils.Dump(frame));

            string body = "";
            string message = "";

            if (frame.Headers.ContainsKey("message")) {

                message = frame.Headers["message"];

            }

            if (frame.Body != null) {

                body = Stomp.ConvertToString(frame.Body, this.Level.ToString());
                body = Regex.Replace(body, @"\r\n?|\n|\r", " ");

            }

            log.ErrorMsg(key.ProtocolError(), message, body);

            log.Trace("Leaving OnError()");

        }

        /// <summary>
        /// Handle a STOMP MESSAGE response.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public override void OnMessage(Frame frame) {

            log.Trace("Entering OnMessage()");

            log.Debug(Utils.Dump(frame));

            log.Trace("Leaving OnMessage()");

        }

        /// <summary>
        /// Handle a STOMP NOOP response.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public override void OnNoop(Frame frame) {

            log.Trace("Entering OnNoop()");

            log.Debug(Utils.Dump(frame));

            log.Trace("Entering OnNoop()");

        }

    }

}
