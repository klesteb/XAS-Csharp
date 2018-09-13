using System;
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

    public delegate void OnDataSent(Int32 id);
    public delegate void OnException(Int32 id, Exception ex);
    public delegate void OnDataReceived(Int32 id, byte[] buffer);

    /// <summary>
    /// A basic async TCP server with ssl.
    /// </summary>
    /// 
    public class Server {

        private Socket listener = null;
        private Object _critical = null;
        private Object _dictionary = null;
        private ManualResetEvent accept = null;
        private ManualResetEvent throttle = null;
        private System.Timers.Timer reaperTimer = null;

        private readonly ILogger log = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;
        private readonly Dictionary<Int32, State> clients = null;

        /// <summary>
        /// Get/Set the name of the host.
        /// </summary>
        /// 
        public String Host { get; set; }

        /// <summary>
        /// Get/Set the port number to listen on.
        /// </summary>
        /// 
        public Int32 Port { get; set; }

        /// <summary>
        /// Get/Set the backlog of available connections.
        /// </summary>
        /// 
        public Int32 Backlog { get; set; }

        /// <summary>
        /// Get/Set the number of client connections.
        /// </summary>
        /// 
        public Int32 MaxConnections { get; set; }

        /// <summary>
        /// Get/Set a timeout for inactive clients.
        /// </summary>
        /// 
        public Int32 ClientTimeout { get; set; }

        /// <summary>
        /// Toggles wither to use SSL.
        /// </summary>
        /// 
        public Boolean UseSSL { get; set; }

        /// <summary>
        /// Gets/Sets the SSL CA cerificate to use.
        /// </summary>
        /// 
        public String SSLcacert { get; set; }

        /// <summary>
        /// Toggles wither to verify the SSL peer.
        /// </summary>
        /// 
        public Boolean SSLVerifyPeer { get; set; }

        /// <summary>
        /// Gets/Sets the SSL Protocols.
        /// </summary>
        /// 
        public SslProtocols SSLProtocols { get; set; }

        /// <summary>
        /// Get/Sets the SSL encryption policy.
        /// </summary>
        /// 
        public EncryptionPolicy SSLEncryptionPolicy { get; set; }

        /// <summary>
        /// Gets/Sets the cancelation token.
        /// </summary>
        /// 
        public CancellationTokenSource Cancellation { get; set; }

        /// <summary>
        /// Get/Set the delegate to call when data has been sent.
        /// </summary>
        /// 
        public OnDataSent OnDataSent { get; set; }

        /// <summary>
        /// Get/Set the delegate to call whan an exception has occurred.
        /// </summary>
        /// 
        public OnException OnException { get; set; }

        /// <summary>
        /// Get/Set the delegate to call when data has been received.
        /// </summary>
        /// 
        public OnDataReceived OnDataReceived { get; set; }

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
            this.Host = "localhost";

            this.UseSSL = false;
            this.SSLcacert = "";
            this.SSLVerifyPeer = false;
            this.SSLEncryptionPolicy = EncryptionPolicy.RequireEncryption;
            this.SSLProtocols = (SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12);

            this.config = config;
            this.handler = handler;
            this._critical = new Object();
            this._dictionary = new Object();
            this.OnException += ExceptionHander;
            this.accept = new ManualResetEvent(false);
            this.throttle = new ManualResetEvent(false);
            this.log = logFactory.Create(typeof(Server));
            this.clients = new Dictionary<Int32, State>();

            this.reaperTimer = new System.Timers.Timer(60 * 1000);
            this.reaperTimer.Elapsed += ReapClients;

        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            reaperTimer.Start();

            IPHostEntry host = Dns.GetHostEntry(Host);
            IPAddress ip = host.AddressList[3];
            IPEndPoint socket = new IPEndPoint(ip, Port);
            var callback = new AsyncCallback(OnClientConnectCallback);          

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(socket);
            listener.Listen(this.Backlog);

            for (;;) {

                accept.Reset();

                if (Cancellation.Token.IsCancellationRequested) {

                    Stop();
                    break;

                }

                if ((clients.Count > MaxConnections) && (MaxConnections > 0)) {

                    throttle.Reset();
                    throttle.WaitOne();

                }

                listener.BeginAccept(callback, listener);

                accept.WaitOne();

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

        #region Private Methods

        private void OnClientConnectCallback(IAsyncResult result) {

            log.Trace("Entering OnClientConnectCallback()");

            var key = config.Key;
            var client = new State();
            var section = config.Section;
            var listener = (Socket)result.AsyncState;
            var callback = new AsyncCallback(OnDataReceivedCallback);

            try {

                client.Connected = true;
                client.Socket = listener.EndAccept(result);

                var remoteEndPoint = listener.RemoteEndPoint as IPEndPoint;

                client.RemotePort = remoteEndPoint.Port;
                client.RemoteHost = remoteEndPoint.Address.ToString();
                client.Activity = DateTime.Now.ToUnixTime();

                if (! Cancellation.Token.IsCancellationRequested) {

                    client.Stream = new NetworkStream(client.Socket);

                    if (UseSSL) {

                        SetSslOptions(client);

                    }

                    AddClient(client);

                    log.InfoMsg(key.ClientConnect(), client.RemoteHost, client.RemotePort);
                    client.Stream.BeginRead(client.Buffer, 0, client.Size, callback, client);

                }

            } catch (AuthenticationException ex) {

                this.OnException(client.Id, ex);

            } catch (ObjectDisposedException) {

                // do nothing, an expected error

            } finally {

                accept.Set();

            }

            log.Trace("Leaving OnClientConnectCallback()");

        }

        private void OnDataReceivedCallback(IAsyncResult result) {

            log.Trace("Entering OnDataReceivedCallback()");

            var client = (State)result.AsyncState;
            var callback = new AsyncCallback(ReadCallback);

            try {

                if (! Cancellation.Token.IsCancellationRequested) {

                    if (client.Connected && (client.Stream != null)) {

                        client.Stream.BeginRead(client.Buffer, 0, client.Size, callback, client);

                    }

                }

            } catch (SocketException ex) {

                this.OnException(client.Id, ex);

            }

            log.Trace("Leaving OnDataReceivedCallback()");

        }

        private void ReadCallback(IAsyncResult asyn) {

            log.Trace("Entering ReadCallback()");

            var client = (State)asyn.AsyncState;
            var callback = new AsyncCallback(OnDataReceivedCallback);

            try {

                client.Count = client.Stream.EndRead(asyn);

                if (client.Count > 0) {

                    byte[] buffer = new byte[client.Count];

                    Array.Copy(client.Buffer, buffer, client.Count);
                    Array.Clear(client.Buffer, 0, client.Size);

                    if (this.OnDataReceived != null) {

                        this.OnDataReceived(client.Id, buffer);

                    }

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

            } catch (NullReferenceException) {

                // ignore, Disconnect() with an outstanding read.

                log.Debug("ReadCallback() - Ignored but a NullReferenceException was thrown");

            } catch (Exception ex) {

                this.OnException(client.Id, ex);

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

                if (this.OnDataSent != null) {

                    this.OnDataSent(client.Id);

                }

            } catch (Exception ex) {

                this.OnException(client.Id, ex);

            }

            log.Trace("Leaving WriteCallback()");

        }

        private void ExceptionHander(Int32 id, Exception ex) {

            var client = GetClient(id);

            if (client != null) {

                string msg = String.Format("Host: {0}, Port: {1} has problems", client.RemoteHost, client.RemoteHost);
                log.Error(msg, ex);

            } else {

                handler.Exceptions(ex);

            }

        }
        
        private void ReapClients(object sender, EventArgs args) {

            log.Trace("Entering ReapClients()");

            // poormans GC
            //
            // there is no good way to tell if a socket is active.
            // you can only tell when you can't do i/o to it.
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
            // dosen't explain, microsofts or otherwise.

            var removals = new List<Int32>();
            Int64 now = DateTime.Now.ToUnixTime();

            foreach (KeyValuePair<Int32, State> client in clients) {

                if (Cancellation.Token.IsCancellationRequested) {

                    return;

                }

                // check for client inactivity

                if (ClientTimeout > 0) {

                    if ((now - client.Value.Activity) > ClientTimeout) {

                        DisconnectClient(client.Value);
                        removals.Add(client.Key);

                        continue;

                    }

                }

                // check for dead sockets

                if (!client.Value.Socket.IsConnected()) {

                    DisconnectClient(client.Value);
                    removals.Add(client.Key);

                }

            }

            foreach (Int32 key in removals) {

                if (this.Cancellation.Token.IsCancellationRequested) {

                    return;

                }

                DeleteClient(key);

            }

            if ((clients.Count < MaxConnections) && (MaxConnections > 0)) {

                throttle.Set();

            }

            log.Trace("Leaving ReapClients()");

        }

        private State CloneClient(State state) {

            var junk = new State(size: state.Size) {
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

            return junk;

        }

        private void AddClient(State state) {

            lock (_dictionary) {

                state.Id = !clients.Any() ? 1 : clients.Keys.Max() + 1;
                clients.Add(state.Id, state);

            }

        }

        private void UpdateClient(State state) {

            lock (_dictionary) {

                clients[state.Id] = state;

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

                state.Socket.Close();
                state.Stream.Close();
                state.Connected = false;

                clients[state.Id] = state;

            }

        }

        private void RemoveClients() {

            lock (_dictionary) {

                foreach (KeyValuePair<Int32, State> client in clients) {

                    client.Value.Stream.Close();
                    client.Value.Socket.Close();

                }

                clients.Clear();

            }

        }

        private void SetSslOptions(State client) {

            log.Trace("Entering SetSslOptions()");

            var sslStream = new SslStream(
                client.Stream,
                false,
                new RemoteCertificateValidationCallback(OnValidateCertificate),
                new LocalCertificateSelectionCallback(SelectLocalCertificate),
                this.SSLEncryptionPolicy
            );

            // this will throw an exception

            if (!String.IsNullOrEmpty(this.SSLcacert)) {

                var serverCertificate = X509Certificate.CreateFromCertFile(this.SSLcacert);
                sslStream.AuthenticateAsServer(serverCertificate, true, false);

            }

            client.Stream = sslStream;

            log.Trace("Leaving SetSslOptions()");

        }

        private bool OnValidateCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors) {

            log.Trace("Entering OnValidateCertificate()");

            bool stat = true;

            if (sslPolicyErrors != SslPolicyErrors.None) {

                stat = this.SSLVerifyPeer;

            }

            log.Trace("Leaving OnValidateCertificate()");

            return stat;

        }

        private X509Certificate SelectLocalCertificate(
            object sender,
            string targetHost,
            X509CertificateCollection localCertificates,
            X509Certificate remoteCertificate,
            string[] acceptableIssuers) {

            log.Trace("Entering SelectLocalCertifcate()");

            X509Certificate ourCertificate = null;

            if ((acceptableIssuers != null) &&
                (acceptableIssuers.Length > 0) &&
                (localCertificates != null) &&
                (localCertificates.Count > 0)) {

                foreach (X509Certificate certificate in localCertificates) {

                    string issuer = certificate.Issuer;

                    if (Array.IndexOf(acceptableIssuers, issuer) != -1) {

                        ourCertificate = certificate;
                        break;

                    }

                }

            }

            if ((localCertificates != null) && (localCertificates.Count > 0)) {

                ourCertificate = localCertificates[0];

            }

            log.Trace("Leaving SelectLocalCertifcate()");

            return ourCertificate;

        }

        #endregion

    }

}
