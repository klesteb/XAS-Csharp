using System;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using XAS.Core;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.Network.Configuration.Extensions;

namespace XAS.Network.TCP {

    /// <summary>
    /// An implementation of a basic TCP network client with SSL support.
    /// </summary>
    /// 
    public class Client: IDisposable {

        private readonly ILogger log = null;
        protected readonly IConfiguration config = null;
        protected readonly IErrorHandler handler = null;
        protected readonly ILoggerFactory logFactory = null;

        private XAS.Network.TCP.Context context = null;
        private AutoResetEvent timeoutEvent = new AutoResetEvent(false);
        private X509CertificateCollection clientCerts = new X509CertificateCollection();

        /// <summary>
        /// Gets/Sets the port number.
        /// </summary>
        /// 
        public Int32 Port { get; set; }

        /// <summary>
        /// Gets/Sets the server.
        /// </summary>
        /// 
        public String Server { get; set; }

        /// <summary>
        /// Gets/Sets the connection timeout.
        /// </summary>
        /// 
        public Int32 Timeout { get; set; }

        /// <summary>
        /// Gets/Sets the receive buffer size.
        /// </summary>
        /// 
        public Int32 BufferSize { get; set; }

        /// <summary>
        /// Gets/Sets wither to use TCP keepalive.
        /// </summary>
        /// 
        public Boolean Keepalive { get; set; }

        /// <summary>
        /// Gets/Sets the TCP keepalive timeout.
        /// </summary>
        /// 
        public UInt32 KeepaliveTimeout { get; set; }

        /// <summary>
        /// Gets/Sets the TCP keepalive interval.
        /// </summary>
        /// 
        public UInt32 KeepaliveInterval { get; set; }

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
        /// Gets wither the connection was successful.
        /// </summary>
        /// 
        public Boolean IsConnectionSuccessful { get; set; }

        /// <summary>
        /// Set the event for handling connections.
        /// </summary>
        /// 
        public event OnClientConnect OnConnect;

        /// <summary>
        /// Set the event for handling when data was sent.
        /// </summary>
        /// <remarks>An internal delegate is called, which does nothing.</remarks>
        /// 
        public event OnClientDataSent OnDataSent;

        /// <summary>
        /// Set the event for handling disconnects.
        /// </summary>
        /// 
        public event OnClientDisconnect OnDisconnect;

        /// <summary>
        /// Set the event for when exceptions happen.
        /// </summary>
        /// <remarks>An internal delegate is called, which invokes ErrorHandler.Exception().</remarks>
        /// 
        public event OnClientException OnException;

        /// <summary>
        /// Set the event for when data is recieved.
        /// </summary>
        /// 
        public event OnClientDataReceived OnDataReceived;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">Am ILoggerFactory object.</param>
        /// 
        public Client(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.logFactory = logFactory;

            this.Port = 7;              // echo-stream server
            this.BufferSize = 1024;
            this.Server = "127.0.0.1";  // use the local network interface

            this.Timeout = 30;

            this.Keepalive = false;
            this.KeepaliveInterval = 5;
            this.KeepaliveTimeout = 360;

            this.UseSSL = false;
            this.SSLcacert = "";
            this.SSLVerifyPeer = false;
            this.SSLEncryptionPolicy = EncryptionPolicy.RequireEncryption;
            this.SSLProtocols = (SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12);

            this.IsConnectionSuccessful = false;
            this.Cancellation = new CancellationTokenSource();

            this.OnConnect += InternalOnConnect;
            this.OnDataSent += InternalOnDataSent;
            this.OnException += InternalOnException;
            this.OnDisconnect += InternalOnDisconnect;

            this.log = logFactory.Create(typeof(Client));

        }

        /// <summary>
        /// Connect to the server.
        /// </summary>
        /// 
        public Boolean Connect() {

            log.Trace("Entering Connect()");

            bool stat = false;
            var key = config.Key;
            var section = config.Section;

            try {

                int timeout = this.Timeout * 1000;
                context = new Context(size: this.BufferSize);
                AsyncCallback connectCallback = new AsyncCallback(ConnectCallback);

                context.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);                
                context.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                context.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                context.Socket.BeginConnect(Server, Port, connectCallback, context);

                if (timeoutEvent.WaitOne(timeout, false)) {

                    if (IsConnectionSuccessful) {

                        log.Debug(String.Format("Connect() - Connected to {0}", Server));

                        if (Keepalive) {

                            context.Socket.SetKeepaliveValues(KeepaliveTimeout, KeepaliveInterval);

                        }

                        context.Connected = true;
                        context.Stream = new NetworkStream(context.Socket);

                        if (UseSSL) {

                            SetSslOptions();

                        }

                        OnConnect();

                        stat = true;

                    }

                } else {

                    throw new TimeoutException(config.GetValue(section.Messages(), key.ConnectionTimeoutException()), null);

                }
                
            } catch (Exception ex) {

                OnException(ex);

            }

            log.Trace("Leaving Connect()");

            return stat;

        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        /// 
        public void Disconnect() {

            log.Trace("Entering Disconnect()");

            if (context.Stream != null) {

                context.Stream.Dispose();
                context.Stream = null;

            }

            if (context.Socket != null) {

                context.Socket.Close();
                context.Socket = null;

            }

            context.Connected = false;
            IsConnectionSuccessful = false;

            log.Trace("Leaving Disconnect()");

        }

        /// <summary>
        /// Reconnect to the server. This provides a simple backoff methodology. 
        /// </summary>
        /// <returns>true on sucess.</returns>
        /// 
        public Boolean Reconnect() {

            log.Trace("Entering Reconnect()");

            bool stat = false;
            var key = config.Key;
            Int32[] retries = { 60, 120, 240, 480, 960, 1920, 3840 };

            log.InfoMsg(key.ServerReconnect(), Server);

            if (! Cancellation.IsCancellationRequested) {

                stat = Retry.UntilTrue(retries, Connect);

            }

            log.Trace("Leaving Reconnect()");

            return stat;

        }

        /// <summary>
        /// Send a buffer to the server.
        /// </summary>
        /// <param name="buffer">A byte array.</param>
        /// 
        public void Send(Byte[] buffer) {

            log.Trace("Entering Send()");

            if (! Cancellation.IsCancellationRequested) {

                if (context.Connected && (context.Stream != null)) {

                    AsyncCallback callback = new AsyncCallback(WriteCallback);
                    context.Stream.BeginWrite(buffer, 0, buffer.Length, callback, context);

                }

            }

            log.Trace("Leaving Send()");

        }

        /// <summary>
        /// Receive a buffer from the server.
        /// </summary>
        /// 
        public void Receive() {

            log.Trace("Entering Receive()");

            if (! Cancellation.IsCancellationRequested) {

                WaitForData();

            }

            log.Trace("Leaving Receive()");

        }

        /// <summary>
        /// Checks wither data is waiting.
        /// </summary>
        /// <returns>True if data is waiting.</returns>
        /// 
        public Boolean DataPending() {

            Boolean stat = false;

            log.Trace("Entering DataPending()");

            if (context.Socket != null) {
            
                stat = (context.Socket.Available > 0);

            }

            log.Trace("Leaving DataPending()");

            return stat;

        }

        #region Delegate Methods

        private void InternalOnConnect() {

            log.Trace("Entering InternalOnConnect()");

            // do nothing

            log.Trace("Leaving InternalOnConnect()");

        }

        private void InternalOnException(Exception ex) {

            log.Trace("Entering InternalOnException()");

            // do nothing

            log.Trace("Leaving InternalOnException()");

        }

        private void InternalOnDataSent() {

            log.Trace("Entering InternalOnDataSent()");

            Receive();

            log.Trace("Leaving InternalOnDataSent()");

        }

        private void InternalOnDisconnect() {

            log.Trace("Entering InternalOnDisconnect()");

            // do nothing

            log.Trace("Leaving InternalOnDisconnect()");

        }

        #endregion
        #region Callback Handling

        private void WaitForData() {

            log.Trace("Entering WaitForData()");

            if (context.Connected && (context.Stream != null)) {

                AsyncCallback callback = new AsyncCallback(ReadCallback);
                context.Stream.BeginRead(context.Buffer, 0, context.Size, callback, context);

            }

            log.Trace("Leaving WaitForData()");

        }

        private void ConnectCallback(IAsyncResult asyn) {

            log.Trace("Entering ConnectCallback()");

            try {

                Context context = (Context)asyn.AsyncState;
                context.Socket.EndConnect(asyn);

                IsConnectionSuccessful = false;

                if (context.Socket.IsConnected()) {

                    IsConnectionSuccessful = true;

                }

            } catch (ObjectDisposedException) {

                // ignore, stream has been disposed with an outstanding read.

                log.Debug("ConnectCallback() - Ignored but a ObjectDisposedException was thrown");

            } catch (NullReferenceException) {

                // ignore, Disconnect() with an outstanding read.

                log.Debug("ConnectCallback() - Ignored but a NullReferenceException was thrown");

            } catch (Exception ex) {

                OnException(ex);

            } finally {

                timeoutEvent.Set();

            }

            log.Debug(String.Format("ConnectCallback() - IsConnectionSuccessful = {0}", this.IsConnectionSuccessful));
            log.Trace("Leaving ConnectCallback()");

        }

        private void ReadCallback(IAsyncResult asyn) {

            log.Trace("Entering ReadCallback()");

            try {

                Context context = (Context)asyn.AsyncState;
                context.Count = context.Stream.EndRead(asyn);

                if (context.Count > 0) {

                    byte[] buffer = new byte[context.Count];
                    Array.Copy(context.Buffer, buffer, context.Count);

                    OnDataReceived(buffer);

                    context.Count = 0;

                } else {

                    IsConnectionSuccessful = false;

                    // stream has been remotely closed.

                    log.Debug("ReadCallback() - read == 0");

                    OnDisconnect();

                }

            } catch (ObjectDisposedException) {

                // ignore, stream has been disposed with an outstanding read.

                log.Debug("ReadCallback() - Ignored but a ObjectDisposedException was thrown");

            } catch (NullReferenceException) {

                // ignore, Disconnect() with an outstanding read.

                log.Debug("ReadCallback() - Ignored but a NullReferenceException was thrown");

            } catch (Exception ex) {

                OnException(ex);

            }

            log.Trace("Leaving ReadCallback()");

        }

        private void WriteCallback(IAsyncResult asyn) {

            log.Trace("Entering WriteCallback()");

            try {

                Context context = (Context)asyn.AsyncState;
                context.Stream.EndWrite(asyn);
                context.Stream.Flush();

                OnDataSent();

            } catch (Exception ex) {

                OnException(ex);

            }

            log.Trace("Leaving WriteCallback()");

        }

        #endregion
        #region SSL Handling

        private void SetSslOptions() {

            log.Trace("Entering SetSslOptions()");

            var sslStream = new SslStream(
                context.Stream, 
                false,
                new RemoteCertificateValidationCallback(OnValidateCertificate),
                new LocalCertificateSelectionCallback(SelectLocalCertificate),
                this.SSLEncryptionPolicy
            );

            if (!String.IsNullOrEmpty(this.SSLcacert)) {

                // comma delimited list ?

                if (this.SSLcacert.Contains(",")) {

                    foreach (string cert in Regex.Split(this.SSLcacert, ",")) {

                        X509Certificate2 cacert = new X509Certificate2(cert);
                        clientCerts.Add(cacert);

                    }

                } else {

                    X509Certificate2 cacert = new X509Certificate2(this.SSLcacert);
                    clientCerts.Add(cacert);

                }

            }

            sslStream.AuthenticateAsClient(this.Server, clientCerts, this.SSLProtocols, true);
            context.Stream = sslStream;

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
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Generic dispose.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    // TODO: dispose managed state (managed objects).

                    this.Disconnect();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                foreach (OnClientConnect item in OnConnect.GetInvocationList()) {

                    OnConnect -= item;
                
                }

                foreach (OnClientDataReceived item in OnDataReceived.GetInvocationList()) {

                    OnDataReceived -= item;

                }

                foreach (OnClientDataSent item in OnDataSent.GetInvocationList()) {

                    OnDataSent -= item;

                }

                foreach (OnClientDisconnect item in OnDisconnect.GetInvocationList()) {

                    OnDisconnect -= item;

                }

                foreach (OnClientException item in OnException.GetInvocationList()) {

                    OnException -= item;

                }

                disposedValue = true;

            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Shares() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// Generic dispose.
        /// </summary>

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
