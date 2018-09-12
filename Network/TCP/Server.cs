using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.Network.Configuration.Extensions;
using System.Threading.Tasks;

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
        private ManualResetEvent mre = null;
        private System.Timers.Timer reaperTimer = null;

        private readonly ILogger log = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;
        private readonly ConcurrentDictionary<Int32, State> clients = null;

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
        /// Get.Set the number of client connections.
        /// </summary>
        /// 
        public Int32 Connections { get; set; }

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

            this.Port = 7;          // echo server
            this.Backlog = 10;
            this.Connections = 0;
            this.Host = "localhost";

            this.UseSSL = false;
            this.SSLcacert = "";
            this.SSLVerifyPeer = false;
            this.SSLEncryptionPolicy = EncryptionPolicy.RequireEncryption;
            this.SSLProtocols = (SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12);

            this.config = config;
            this.handler = handler;
            this.OnException += ExceptionHander;
            this.mre = new ManualResetEvent(false);
            this.log = logFactory.Create(typeof(Server));
            this.clients = new ConcurrentDictionary<Int32, State>();

            this.reaperTimer = new System.Timers.Timer(60 * 1000);
            this.reaperTimer.Elapsed += ClientReaper;

        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            reaperTimer.Start();

            IPHostEntry host = Dns.GetHostEntry(this.Host);
            IPAddress ip = host.AddressList[3];
            IPEndPoint socket = new IPEndPoint(ip, this.Port);

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(socket);
            listener.Listen(this.Backlog);

            for (;;) {

                mre.Set();

                if (Cancellation.Token.IsCancellationRequested) {

                    Stop();
                    break;

                }

                if ((clients.Count > Connections) && (Connections != 0)) {

                    // throttle client connections. the reaper will remove 
                    // dead clients and clear this event flag.

                    mre.WaitOne();

                }

                listener.BeginAccept(new AsyncCallback(OnClientConnect), listener);

                mre.WaitOne();

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

            foreach (KeyValuePair<Int32, State> client in clients) {

                client.Value.Stream.Close();
                client.Value.Socket.Close();
                
            }

            clients.Clear();
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
        /// <returns>A State object for the client.</returns>
        /// 
        public State GetClient(Int32 id) {

            var state = new State();
            return clients.TryGetValue(id, out state) ? state : null;

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

        private void OnClientConnect(IAsyncResult result) {

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

                if (! Cancellation.Token.IsCancellationRequested) {

                    client.Id = !clients.Any() ? 1 : clients.Keys.Max() + 1;
                    clients.TryAdd(client.Id, client);

                    client.Stream = new NetworkStream(client.Socket);

                    if (this.UseSSL) {

                        SetSslOptions(client);

                    }

                    log.InfoMsg(key.ClientConnect(), client.RemoteHost, client.RemotePort);
                    client.Stream.BeginRead(client.Buffer, 0, client.Size, callback, client);

                }

            } catch (AuthenticationException ex) {

                this.OnException(client.Id, ex);

            } catch (ObjectDisposedException) {

                // do nothing, an expected error

            } finally {

                mre.Set();

            }

        }

        private void OnDataReceivedCallback(IAsyncResult result) {

            var client = (State)result.AsyncState;
            var callback = new AsyncCallback(ReadCallback);

            try {

                client.Count = client.Socket.EndReceive(result);

                if (! Cancellation.Token.IsCancellationRequested) {

                    if (client.Stream != null) {

                        client.Stream.BeginRead(client.Buffer, 0, client.Size, callback, client);

                    }

                }

            } catch (SocketException ex) {

                this.OnException(client.Id, ex);

            }

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
                    client.Stream.BeginRead(client.Buffer, 0, client.Size, callback, client);

                } else {

                    // stream has been remotely closed
                    // let the reaper take care of the dead client

                    log.Debug("ReadCallback() - read == 0");

                    var junk = new State(size: client.Size) {
                        Count = 0,
                        Id = client.Id,
                        Connected = false,
                        Close = client.Close,
                        Stream = client.Stream,
                        Socket = client.Socket,
                        RemoteHost = client.RemoteHost,
                        RemotePort = client.RemotePort,
                    };

                    clients.TryUpdate(client.Id, client, junk);

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
        
        private void ClientReaper(object sender, EventArgs args) {

            var junk = new State();
            var removals = new List<Int32>();

            foreach (KeyValuePair<Int32, State> client in clients) {

                if (Cancellation.Token.IsCancellationRequested) {

                    return;

                }

                if (! client.Value.Socket.IsConnected()) {

                    client.Value.Socket.Close();
                    client.Value.Stream.Close();

                    removals.Add(client.Key);

                }

            }

            foreach (Int32 key in removals) {

                if (this.Cancellation.Token.IsCancellationRequested) {

                    return;

                }

                clients.TryRemove(key, out junk);

            }

            if ((clients.Count < Connections) && (Connections != 0)) {

                mre.Set();

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
