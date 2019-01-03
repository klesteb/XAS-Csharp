using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.Network.Configuration.Extensions;

namespace XAS.Network.STOMP {

    /// <summary>
    /// A STOMP Client class.
    /// </summary>
    //
    public class Client: TCP.Client {

        private Parser parser = null;
        private Task dispatchTask = null;
        private readonly ILogger log = null;
        private AutoResetEvent dispatchEvent = null;
        private ConcurrentQueue<Frame> frames = null;

        protected readonly Stomp stomp = null;

        /// <summary>
        /// Gets/Sets the STOMP version level.
        /// </summary>
        /// 
        public Single Level { get; set; }

        /// <summary>
        /// Gets/Sets the username to connect to the server.
        /// </summary>
        /// 
        public String Username { get; set; }

        /// <summary>
        /// Gets/Sets the password for the username.
        /// </summary>
        /// 
        public String Password { get; set; }

        /// <summary>
        /// Gets/Sets the virtual host to use on the server.
        /// </summary>
        /// 
        public String VirtualHost { get; set; }

        /// <summary>
        /// Gets/Sets the subscription for the STOMP session.
        /// </summary>
        /// 
        public String Subscription { get; set; }

        /// <summary>
        /// Gets/Sets the STOMP protocol heartbeat.
        /// </summary>
        /// 
        public String Heartbeat {  get; set; }
        /// <summary>
        /// Set the event handler for handling a STOMP Connected frame.
        /// </summary>
        /// 
        public event StompConnectedHandler OnStompConnected;

        /// <summary>
        /// Set the event handler for handling a STOMP Message frame.
        /// </summary>
        /// 
        public event StompMessageHandler OnStompMessage;

        /// <summary>
        /// Set the event handler for handling a STOMP Receipt frame.
        /// </summary>
        /// 
        public event StompReceiptHandler OnStompReceipt;

        /// <summary>
        /// Set the event handler for handling s STOMP Error frame.
        /// </summary>
        /// 
        public event StompErrorHandler OnStompError;

        /// <summary>
        /// Set the event handler for handling a STOMP NOOP frame.
        /// </summary>
        /// 
        public event StompNoopHandler OnStompNoop;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// <param name="level">The STOMP level to use, defaults to 1.0.</param>
        /// 
        public Client(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, String level = "1.0"): 
            base(config, handler, logFactory) {

            var key = config.Key;
            var section = config.Section;

            parser = new Parser(level: level);
            frames = new ConcurrentQueue<Frame>();
            stomp = new Stomp(config, handler, logFactory);

            this.Heartbeat = "0,0";
            this.VirtualHost = "/";
            this.Username = "guest";
            this.Password = "guest";
            this.Subscription = "stomp-client";
            this.Level = Convert.ToSingle(level);
            this.Cancellation = new CancellationTokenSource();
            this.Server = config.GetValue(section.Environment(), key.MQServer());
            this.Port = Convert.ToInt32(config.GetValue(section.Environment(), key.MQPort()));

            this.dispatchEvent = new AutoResetEvent(false);

            this.OnClientConnect += OnConnect;
            this.OnClientException += OnException;
            this.OnClientDisconnect += OnDisconnect;
            this.OnClientDataReceived += OnDataReceived;

            log = logFactory.Create(typeof(Client));

        }

        /// <summary>
        /// Send a STOMP frame to the server.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public void Send(Frame frame) {

            log.Debug(frame.ToString());

            Send(frame.ToArray());

        }

        #region Internal Event Handlers

        /// <summary>
        /// Handles the callback for an exception.
        /// </summary>
        /// <param name="ex">An Exception object.</param>
        /// 
        public void OnException(Exception ex) {

            log.Trace("Entering OnException()");

            handler.Exceptions(ex);

            log.Trace("Leaving OnException()");

        }

        /// <summary>
        /// Handles the callback for the initial connection to the server.
        /// </summary>
        /// 
        public void OnConnect() {

            log.Trace("Entering OnConnect()");

            var key = config.Key;
            
            log.InfoMsg(key.ServerConnect(), Server, Port);

            StartDispatch();
            Send(
                stomp.Connect(
                    login: Username,
                    passcode: Password,
                    virtualhost: VirtualHost,
                    acceptable: Level.ToString(),
                    level: Level.ToString(),
                    heartbeat: Heartbeat
                )
            );

            log.Trace("Leaving OnConnect()");

        }

        /// <summary>
        /// Handles the callback for when data is received from the network.
        /// </summary>
        /// <param name="buffer">The byte array read from the socket.</param>
        /// 
        public void OnDataReceived(Byte[] buffer) {

            log.Trace("Entering OnDataReceived()");

            Frame frame = null;
            parser.Level = Level;
            parser.Buffer = buffer;

            while ((frame = parser.Filter()) != null) {

                frames.Enqueue(frame);
                dispatchEvent.Set();

            }

            Receive();

            log.Trace("Leaving OnDataReceived()");

        }

        /// <summary>
        /// Handles the callback for when the network is disconnected.
        /// </summary>
        /// 
        public void OnDisconnect() {

            log.Trace("Entering OnDisconnect()");

            var key = config.Key;

            log.WarnMsg(key.ServerDisconnect(), Server);

            StopDispatch();

            log.Trace("Leaving OnDisconnect()");

        }

        #endregion
        #region Private Methods

        private void StartDispatch() {

            log.Trace("Entering StartDispatch()");

            dispatchTask = new Task(Dispatch, Cancellation.Token, TaskCreationOptions.LongRunning);
            dispatchTask.Start();

            log.Trace("Leaving StartDispatch()");

        }

        private void StopDispatch() {

            log.Trace("Entering StopDispatch()");

            if (dispatchTask != null) {

                Task[] tasks = { dispatchTask };

                Cancellation.Cancel(true);
                dispatchEvent.Set();

                Task.WaitAll(tasks);

            }

            log.Trace("Leaving StopDispatch()");

        }

        private void Dispatch() {

            log.Trace("Entering Dispatch()");

            for (;;) {

                dispatchEvent.WaitOne();

                Frame frame;

                if (Cancellation.Token.IsCancellationRequested) {

                    log.Debug("Dispatch() - cancellation requested");
                    break;

                }

                while (frames.TryDequeue(out frame)) {

                    if (Cancellation.Token.IsCancellationRequested) {

                        log.Debug("Dispatch() - cancellation requested");
                        break;

                    }

                    switch (frame.Command) {
                        case "CONNECTED":
                            log.Debug("Dispatch() - received a \"CONNECTED\" message");
                            OnStompConnected(frame);
                            break;

                        case "MESSAGE":
                            log.Debug("Dispatch() - received a \"MESSAGE\" message");
                            OnStompMessage(frame);
                            break;

                        case "RECEIPT":
                            log.Debug("Dispatch() - received a \"RECEIPT\" message");
                            OnStompReceipt(frame);
                            break;

                        case "ERROR":
                            log.Debug("Dispatch() - received a \"ERROR\" message");
                            OnStompError(frame);
                            break;

                        default:
                            log.Debug("Dispatch() - received a \"NOOP\" message");
                            OnStompNoop(frame);
                            break;

                    }

                }

            }

            log.Trace("Leaving Dispatch()");

        }

        #endregion
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Generic dispose.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected override void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    dispatchEvent.Dispose();
                    Task.WaitAny(dispatchTask);

                    foreach (StompConnectedHandler item in OnStompConnected.GetInvocationList()) {

                        OnStompConnected -= item;

                    }

                    foreach (StompErrorHandler item in OnStompError.GetInvocationList()) {

                        OnStompError -= item;

                    }

                    foreach (StompMessageHandler item in OnStompMessage.GetInvocationList()) {

                        OnStompMessage -= item;

                    }

                    foreach (StompNoopHandler item in OnStompNoop.GetInvocationList()) {

                        OnStompNoop -= item;

                    }

                    foreach (StompReceiptHandler item in OnStompReceipt.GetInvocationList()) {


                        OnStompReceipt -= item;

                    }

                }

                disposedValue = true;
                base.Dispose(disposing);

            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Shares() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        // public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            // Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        // }

        #endregion

    }

}
