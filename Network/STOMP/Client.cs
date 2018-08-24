using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

namespace XAS.Network.STOMP {

    /// <summary>
    /// A STOMP Client class.
    /// </summary>
    //
    public abstract class Client: TCP.Client, IDisposable {

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
        /// Constructor.
        /// </summary>
        /// <param name="level">The STOMP version level to use.</param>
        /// 
        public Client(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, String level = "1.0"): 
            base(config, handler, logFactory) {

            var key = config.Key;
            var section = config.Section;

            parser = new Parser(level: level);
            frames = new ConcurrentQueue<Frame>();
            stomp = new Stomp(config, handler, logFactory);

            this.VirtualHost = "/";
            this.Username = "guest";
            this.Password = "guest";
            this.Subscription = "stomp-client";
            this.Level = Convert.ToSingle(level);
            this.Cancellation = new CancellationTokenSource();
            this.Server = config.GetValue(section.Environment(), key.MQServer());
            this.Port = Convert.ToInt32(config.GetValue(section.Environment(), key.MQPort()));

            this.dispatchEvent = new AutoResetEvent(false);

            log = logFactory.Create(typeof(Client));

        }

        /// <summary>
        /// Send a STOMP frame to the server.
        /// </summary>
        /// <param name="frame">A STOMP frame.</param>
        /// 
        public void Send(Frame frame) {

            log.Debug(frame.ToString());

            this.Send(frame.ToArray());

        }

        #region Abstract Methods

        /// <summary>
        /// Abstract method for the OnConnected event.
        /// </summary>
        /// <param name="frame">A STOMP connected frame.</param>
        ///
        public abstract void OnConnected(Frame frame);

        /// <summary>
        /// Abstract method for the OnMessage event.
        /// </summary>
        /// <param name="frame">A STOMP message frame.</param>
        /// 
        public abstract void OnMessage(Frame frame);

        /// <summary>
        /// Abstract method for the OnReceipt event.
        /// </summary>
        /// <param name="frame">A STOMP receipt frame.</param>
        /// 
        public abstract void OnReceipt(Frame frame);

        /// <summary>
        /// Abstract method for the OnError event.
        /// </summary>
        /// <param name="frame">A STOMP error frame.</param>
        /// 
        public abstract void OnError(Frame frame);

        /// <summary>
        /// Abstract method for the OnNoop event.
        /// </summary>
        /// <param name="frame">A STOMP keepalive frame.</param>
        /// 
        public abstract void OnNoop(Frame frame);

        #endregion
        #region Override Methods

        /// <summary>
        /// Handles the callback for the initial connection to the server.
        /// </summary>
        /// <remarks>
        /// Handles the abstract method from TCP.Client.
        /// </remarks>
        /// 
        public override void OnConnect() {

            log.Trace("Entering OnConnect()");

            this.StartDispatch();
            this.Receive();
            this.Send(
                stomp.Connect(
                    login: this.Username,
                    passcode: this.Password,
                    virtualhost: this.VirtualHost,
                    acceptable: this.Level.ToString(),
                    level: this.Level.ToString()
                )
            );

            log.Trace("Leaving OnConnect()");

        }

        /// <summary>
        /// Handles the callback for when data is written to the network.
        /// </summary>
        /// <remarks>
        /// Handles the abstract method from TCP.Client.
        /// </remarks>
        /// 
        public override void OnDataSent() {

            log.Trace("Entering OnDataSent()");


            log.Trace("Leaving OnDataSent()");

        }

        /// <summary>
        /// Handles the callback for when data i received from the network.
        /// </summary>
        /// <remarks>
        /// Handles the abstract method from TCP.Client.
        /// </remarks>
        /// <param name="buffer">The byte array read from the socket.</param>
        /// 
        public override void OnDataReceived(Byte[] buffer) {

            log.Trace("Entering OnDataReceived()");

            Frame frame = null;
            parser.Buffer = buffer;
            parser.Level = this.Level;

            while ((frame = parser.Filter()) != null) {

                frames.Enqueue(frame);
                dispatchEvent.Set();

            }

            log.Trace("Leaving OnDataReceived()");

        }

        /// <summary>
        /// Handles the callback for when the network is disconnected.
        /// </summary>
        /// <remarks>
        /// Handles the abstract method from TCP.Client.
        /// </remarks>
        /// 
        public override void OnDisconnect() {

            log.Trace("Entering OnDisconnect()");

            this.Cancellation.Cancel(true);
            this.StopDispatch();

            log.Trace("Leaving OnDisconnect()");

        }

        /// <summary>
        /// Handles the callback for when an exception happens in the TCP.Client layer.
        /// </summary>
        /// <remarks>
        /// Handles the abstract method from TCP.Client.
        /// </remarks>
        /// <param name="ex">The thrown exception.</param>
        /// 
        public override void OnException(Exception ex) {

            log.Trace("Entering OnException()");

            handler.Exceptions(ex);

            log.Trace("Leaving OnException()");

        }

        #endregion
        #region Private Methods

        private void StartDispatch() {

            log.Trace("Entering StartDispatch()");

            this.dispatchTask = new Task(this.Dispatch, this.Cancellation.Token, TaskCreationOptions.LongRunning);
            this.dispatchTask.Start();

            log.Trace("Leaving StartDispatch()");

        }

        private void StopDispatch() {

            log.Trace("Entering StopDispatch()");

            Task[] tasks = { this.dispatchTask };

            this.dispatchEvent.Set();
            Task.WaitAll(tasks);

            log.Trace("Leaving StopDispatch()");

        }

        private void Dispatch() {

            log.Trace("Entering Dispatch()");

            for (;;) {

                dispatchEvent.WaitOne();

                Frame frame;

                if (this.Cancellation.Token.IsCancellationRequested) {

                    log.Debug("Dispatch() - cancellation requested");
                    break;

                }

                while (frames.TryDequeue(out frame)) {

                    if (this.Cancellation.Token.IsCancellationRequested) {

                        log.Debug("Dispatch() - cancellation requested");
                        break;

                    }

                    switch (frame.Command) {
                        case "CONNECTED":
                            log.Debug("Dispatch() - received a \"CONNECTED\" message");
                            this.OnConnected(frame);
                            break;

                        case "MESSAGE":
                            log.Debug("Dispatch() - received a \"MESSAGE\" message");
                            this.OnMessage(frame);
                            break;

                        case "RECEIPT":
                            log.Debug("Dispatch() - received a \"RECEIPT\" message");
                            this.OnReceipt(frame);
                            break;

                        case "ERROR":
                            log.Debug("Dispatch() - received a \"ERROR\" message");
                            this.OnError(frame);
                            break;

                        default:
                            log.Debug("Dispatch() - received a \"NOOP\" message");
                            this.OnNoop(frame);
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

        protected override void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    // TODO: dispose managed state (managed objects).

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                base.Dispose(disposing);
                dispatchEvent.Dispose();
                Task.WaitAny(dispatchTask);

                disposedValue = true;

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
