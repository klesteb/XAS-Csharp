using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.Network.Configuration.Extensions;

namespace XAS.Network.TCP {

    /// <summary>
    /// A basic async TCP/IP server with SSL.
    /// </summary>
    /// 
    public class Server: IDisposable {

        private string sslCaCert = "";
        private Socket listener = null;
        private Object _critical = null;
        private Object _dictionary = null;
        private ManualResetEventSlim accept = null;
        private ManualResetEventSlim throttle = null;
        private System.Timers.Timer reaperTimer = null;

        private readonly ILogger log = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;
        private readonly Dictionary<Int32, State> clients = null;

        /// <summary>
        /// Get/Set the the address.
        /// </summary>
        /// 
        public String Address { get; set; }

        /// <summary>
        /// Get/Set the port number to listen on.
        /// </summary>
        /// 
        public Int32 Port { get; set; }

        /// <summary>
        /// Get/Set the backlog of available connections, default is 10.
        /// </summary>
        /// <value>
        /// This is the number of connections.
        /// </value>
        /// 
        public Int32 Backlog { get; set; }

        /// <summary>
        /// Get/Set the number of client connections, default is 0 or unlimitied.
        /// </summary>
        /// 
        public Int32 MaxConnections { get; set; }

        /// <summary>
        /// Get/Set a timeout for inactive clients, default is 0 or unlimitied.
        /// </summary>
        /// 
        public Int32 ClientTimeout { get; set; }

        /// <summary>
        /// Get/Set the interval between client reaper processing, defaults to 60 seconds.
        /// </summary>
        /// 
        public Int32 ReaperInterval { get; set; }

        /// <summary>
        /// Toggles wither to use SSL, default is false.
        /// </summary>
        /// 
        public Boolean UseSSL { get; set; }

        /// <summary>
        /// Gets/Sets the SSL CA Cerificate to use, default is none.
        /// </summary>
        /// <value>
        /// This is a path/name to the CA Certificate.
        /// </value>
        /// 
        public String SSLCaCert { get; set; }

        /// <summary>
        /// Toggles wither to verify the SSL peer, default is false.
        /// </summary>
        /// 
        public Boolean SSLVerifyPeer { get; set; }

        /// <summary>
        /// Gets/Sets the SSL Protocols, default is to allow ssl3, tls1.1 and tls1.2.
        /// </summary>
        /// 
        public SslProtocols SSLProtocols { get; set; }

        /// <summary>
        /// Gets/Sets the cancelation token.
        /// </summary>
        /// 
        public CancellationTokenSource Cancellation { get; set; }

        /// <summary>
        /// Set the event handler to call when data has been sent.
        /// </summary>
        /// 
        public event ServerDataSentHandler OnServerDataSent;

        /// <summary>
        /// Set the event handler to call whan an exception has occurred.
        /// </summary>
        /// <remarks>
        /// A internal exception handler is enbled. It logs the exception.
        /// </remarks>
        /// 
        public event ServerExceptionHandler OnServerException;

        /// <summary>
        /// Set the event handler to call when data has been received.
        /// </summary>
        /// 
        public event ServerDataReceivedHandler OnServerDataReceived;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Server(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.Port = 7;          // a default "echo" server
            this.Backlog = 10;
            this.ClientTimeout = 0;
            this.MaxConnections = 0;
            this.ReaperInterval = 60;
            this.Address = "127.0.0.1";

            this.UseSSL = false;
            this.SSLCaCert = "";
            this.SSLVerifyPeer = false;
            this.SSLProtocols = (SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12);

            this.config = config;
            this.handler = handler;
            this._critical = new Object();
            this._dictionary = new Object();
            this.log = logFactory.Create(typeof(Server));
            this.clients = new Dictionary<Int32, State>();
            this.accept = new ManualResetEventSlim(false);
            this.throttle = new ManualResetEventSlim(false);

            this.OnServerDataSent += InternalOnDataSent;
            this.OnServerException += InternalOnException;
            this.OnServerDataReceived += InternalOnDataReceived;

            this.reaperTimer = new System.Timers.Timer(ReaperInterval * 1000);
            this.reaperTimer.Elapsed += ReapClients;

        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            reaperTimer.Start();

            IPEndPoint socket = new IPEndPoint(IPAddress.Parse(Address), Port);
            var callback = new AsyncCallback(OnAcceptCallback);

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(socket);
            listener.Listen(Backlog);

            for (;;) {

                accept.Reset();

                if (Cancellation.Token.IsCancellationRequested) {

                    Stop();
                    break;

                }

                if ((clients.Count > MaxConnections) && (MaxConnections > 0)) {

                    throttle.Reset();
                    throttle.Wait(Cancellation.Token);

                }

                listener.BeginAccept(callback, listener);
                accept.Wait(Cancellation.Token);

            }

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            reaperTimer.Stop();
            RemoveClients();
            listener.Close();

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause the server.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");

            Stop();

            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Resume the server.
        /// </summary>
        /// 
        public void Resume() {

            log.Trace("Entering Resume()");

            Start();

            log.Trace("Leaving Resume()");

        }

        /// <summary>
        /// Shutdown the server.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            Stop();

            log.Trace("Leaving Shutdown()");

        }

        /// <summary>
        /// Send a buffer to the client.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="buffer">The buffer to send.</param>
        /// 
        public void Send(int id, byte[] buffer) {

            var client = GetClient(id);
            var callback = new AsyncCallback(WriteCallback);

            if (! Cancellation.Token.IsCancellationRequested) {

                if (client != null) {

                    if (client.Connected && (client.Stream != null)) {

                        client.Stream.BeginWrite(buffer, 0, buffer.Length, callback, client);

                    }

                }

            }

        }

        #region Callback Methods

        private void OnAcceptCallback(IAsyncResult result) {

            log.Trace("Entering OnAcceptCallback()");

            var key = config.Key;
            var client = new State();
            var section = config.Section;
            var listener = (Socket)result.AsyncState;
            var callback = new AsyncCallback(ReadCallback);

            try {

                client.Connected = true;
                client.Socket = listener.EndAccept(result);

                var remoteEndPoint = client.Socket.RemoteEndPoint as IPEndPoint;

                client.RemotePort = remoteEndPoint.Port;
                client.RemoteHost = remoteEndPoint.Address.ToString();
                client.Activity = DateTime.Now.ToUnixTime();

                if (! Cancellation.Token.IsCancellationRequested) {

                    client.Stream = new NetworkStream(client.Socket);

                    if (UseSSL) {

                        EnableSSL(client);

                    }

                    AddClient(client);

                    log.InfoMsg(key.ClientConnect(), client.RemoteHost, client.RemotePort);
                    client.Stream.BeginRead(client.Buffer, 0, client.Size, callback, client);

                }

            } catch (AuthenticationException) {

                log.WarnMsg(key.ClientSSLValidation(), client.RemoteHost, client.RemotePort);

                client.Socket.Close();
                client.Stream.Close();

            } catch (ObjectDisposedException) {

                // do nothing, an expected error

                log.Debug("OnAcceptCallback() - Ignored but a ObjectDisposedException was thrown");

            } finally {

                accept.Set();

            }

            log.Trace("Leaving OnAcceptCallback()");

        }

        private void ReadCallback(IAsyncResult asyn) {

            log.Trace("Entering ReadCallback()");

            var client = (State)asyn.AsyncState;
            var callback = new AsyncCallback(ReadCallback);

            try {

                client.Count = client.Stream.EndRead(asyn);

                if (client.Count > 0) {

                    byte[] buffer = new byte[client.Count];

                    Array.Copy(client.Buffer, buffer, client.Count);
                    Array.Clear(client.Buffer, 0, client.Size);

                    OnServerDataReceived(client.Id, buffer);

                    client.Count = 0;
                    client.Activity = DateTime.Now.ToUnixTime();
                    UpdateClient(client);

                    client.Stream.BeginRead(client.Buffer, 0, client.Size, callback, client);

                } else {

                    // stream has been remotely closed
                    // let the reaper take care of the dead client

                    log.Debug("ReadCallback() - read == 0");

                    client.Connected = false;
                    UpdateClient(client);

                }

            } catch (ObjectDisposedException) {

                // ignore, stream has been disposed with an outstanding read.

                log.Debug("ReadCallback() - Ignored but a ObjectDisposedException was thrown");

            } catch (IOException) {

                // ignore, stream has been disposed with an outstanding read.

                log.Debug("ReadCallback() - Ignored but a IOException was thrown");

            } catch (NullReferenceException) {

                // ignore, remote disconnect with an outstanding read.

                log.Debug("ReadCallback() - Ignored but a NullReferenceException was thrown");

            } catch (Exception ex) {

                OnServerException(client.Id, ex);

            }

            log.Trace("Leaving ReadCallback()");

        }

        private void WriteCallback(IAsyncResult asyn) {

            log.Trace("Entering WriteCallback()");

            var client = (State)asyn.AsyncState;

            try {

                client.Stream.EndWrite(asyn);
                client.Stream.Flush();

                client.Activity = DateTime.Now.ToUnixTime();
                UpdateClient(client);

                OnServerDataSent(client.Id);

            } catch (Exception ex) {

                OnServerException(client.Id, ex);

            }

            log.Trace("Leaving WriteCallback()");

        }

        #endregion
        #region Internal Event Handlers

        private void InternalOnDataSent(Int32 clientId) {

            log.Trace("Entering InternOnDataSent()");

            // do nothing

            log.Trace("Leaving InternalOnDataSent()");

        }

        private void InternalOnException(Int32 id, Exception ex) {

            log.Trace("Entering InternalOnException()");

            var key = config.Key;
            var client = GetClient(id);
            var section = config.Section;

            if (client != null) {

                string format = config.GetValue(section.Messages(), key.ClientProblems());
                string msg = String.Format(format, client.RemoteHost, client.RemotePort);

                log.Error(msg, ex);

            } else {

                handler.Exceptions(ex);

            }

            log.Trace("Leaving InternalOnException()");

        }

        private void InternalOnDataReceived(Int32 clientId, Byte[] buffer) {

            log.Trace("Entering InternalOnDataReceived()");

            // do nothing

            log.Trace("Leaving InternalOnDataReceived()");

        }

        #endregion
        #region Private Methods

        private void ReapClients(object sender, EventArgs args) {

            log.Trace("Entering ReapClients()");

            // poormans GC
            //
            // there is no good way to tell if a socket is active.
            // you can only tell when you can't do any i/o to it.
            //
            // this method will scan the client list and check on
            // the connections. it does it in 2 ways. 
            //
            //   1) if client idle detection has been activated,
            //      has there been any i/o during the timeout period
            //
            //   2) can you write to the socket. 
            //
            // if either of these 2 tests fail, the socket and
            // associated stream are closed and the client removed 
            // from the client list. 
            //
            // if client throttling has been activated this will set 
            // the event flag to start accepting connections again. 
            //
            // if you don't do something like this, you will eventually
            // run out of system resources. something the documentation
            // and examples don't explain, microsofts or otherwise.

            var key = config.Key;
            var keys = new List<Int32>();
            var removals = new List<Int32>();
            Int64 now = DateTime.Now.ToUnixTime();

            // collect all of the client ids

            foreach (Int32 id in clients.Keys.ToList()) {

                keys.Add(id);

            }

            // start processing the clients

            foreach (Int32 id in keys) {

                if (Cancellation.Token.IsCancellationRequested) {

                    return;

                }

                var client = GetClient(id);

                // check for client inactivity

                if (ClientTimeout > 0) {

                    if ((now - client.Activity) > ClientTimeout) {

                        log.WarnMsg(key.ClientInactive(), client.RemoteHost, client.RemotePort);

                        DisconnectClient(client);
                        removals.Add(id);

                        continue;

                    }

                }

                // check for dead sockets

                if (! client.Socket.IsConnected()) {

                    log.WarnMsg(key.ClientDeadSocket(), client.RemoteHost, client.RemotePort);

                    DisconnectClient(client);
                    removals.Add(id);

                }

            }

            // remove any dead clients

            foreach (Int32 id in removals) {

                if (Cancellation.Token.IsCancellationRequested) {

                    return;

                }

                DeleteClient(id);

            }

            // if throttling is activated, restart accepting connections

            if ((clients.Count < MaxConnections) && (MaxConnections > 0)) {

                throttle.Set();

            }

            log.Trace("Leaving ReapClients()");

        }

        #endregion
        #region Client Management

        /// <summary>
        /// Get a client.
        /// </summary>
        /// <param name="id">Client ie.</param>
        /// <returns>The State object for the client or null.</returns>
        /// 
        public State GetClient(Int32 id) {

            lock (_dictionary) {

                var state = new State();
                return clients.TryGetValue(id, out state) ? state : null;

            }

        }

        private State CloneClient(State state) {

            var clone = new State(size: state.Size) {
                Id = state.Id,
                Count = state.Count,
                Close = state.Close,
                Stream = state.Stream,
                Socket = state.Socket,
                Buffer = state.Buffer,
                Connected = state.Connected,
                RemoteHost = state.RemoteHost,
                RemotePort = state.RemotePort,
            };

            return clone;

        }

        private void AddClient(State state) {

            lock (_dictionary) {

                state.Id = !clients.Any() ? 1 : clients.Keys.Max() + 1;
                clients.Add(state.Id, state);

            }

        }

        private void UpdateClient(State state) {

            lock (_dictionary) {

                if (clients.ContainsKey(state.Id)) {

                    clients[state.Id] = state;

                }

            }

        }

        private void DeleteClient(Int32 key) {

            lock (_dictionary) {

                if (clients.ContainsKey(key)) {

                    clients.Remove(key);

                }

            }

        }

        private void DisconnectClient(State state) {

            lock (_dictionary) {

                if (clients.ContainsKey(state.Id)) {

                    try {   // ignore any errors

                        state.Socket.Close();
                        state.Stream.Close();

                    } catch { }

                    state.Connected = false;
                    clients[state.Id] = state;

                }

            }

        }

        private void RemoveClients() {

            lock (_dictionary) {

                foreach (KeyValuePair<Int32, State> client in clients) {

                    try {   // ignore any errors

                        client.Value.Stream.Close();
                        client.Value.Socket.Close();

                    } catch { }

                }

                clients.Clear();

            }

        }

        #endregion
        #region SSL Support

        private void EnableSSL(State state) {

            log.Trace("Entering EnableSSL()");

            // this will throw an exception if unable to validate

            var sslStream = new SslStream(state.Stream, false);

             var serverCertificate = X509Certificate.CreateFromCertFile(this.sslCaCert);    
            sslStream.AuthenticateAsServer(serverCertificate, this.SSLVerifyPeer, this.SSLProtocols, true);

            state.Stream = sslStream;

            log.Trace("Leaving EnableSSL()");

        }

        #endregion
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    listener.Close();
                    RemoveClients();

                    foreach (ServerDataReceivedHandler item in OnServerDataReceived.GetInvocationList()) {

                        OnServerDataReceived -= item;

                    }

                    foreach (ServerDataSentHandler item in OnServerDataSent.GetInvocationList()) {

                        OnServerDataSent -= item;

                    }

                    foreach (ServerExceptionHandler item in OnServerException.GetInvocationList()) {

                        OnServerException -= item;

                    }

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;

            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Server() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion

    }

}
