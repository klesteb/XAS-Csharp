using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using FluentFTP;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace XAS.Network.FTP {

    /// <summary>
    /// A FTP Client class.
    /// </summary>
    /// 
    public class Client: IDisposable {

        private readonly ILogger log = null;
        private readonly FtpClient client = null;
        protected readonly IConfiguration config = null;
        protected readonly IErrorHandler handler = null;
        protected readonly ILoggerFactory logFactory = null;

        /// <summary>
        /// Gets/Sets wither to overwrite an existing file.
        /// </summary>
        /// 
        public Boolean OverWrite { get; set; }

        /// <summary>
        /// Gets/Sets wither to verify the peer.
        /// </summary>
        /// 
        public Boolean VerifyPeer { get; set; }

        /// <summary>
        /// Gets/Sets wither to create the remote directory.
        /// </summary>
        /// 
        public Boolean CreateRemoteDirectory { get; set; }

        /// <summary>
        /// Gets/Sets the host that you are connecting too.
        /// </summary>
        /// 
        public String Host { get; set; }

        /// <summary>
        /// Gets/Sets the port on the host to connect too.
        /// </summary>
        /// 
        public Int32 Port { get; set; }

        /// <summary>
        /// Gets/Sets the username when connecting to the host.
        /// </summary>
        /// 
        public String Username { get; set; }

        /// <summary>
        /// Gets/Sets the password for the username.
        /// </summary>
        /// 
        public String Password { get; set; }

        /// <summary>
        /// Gets/Sets wither to use SSL on the connection.
        /// </summary>
        /// 
        public Boolean Secure { get; set; }

        /// <summary>
        /// Gets/Sets a SSL certificate to use.
        /// </summary>
        /// 
        public String SSLcacert { get; set; }

        /// <summary>
        /// Gets/Sets the timeout on connections.
        /// </summary>
        /// 
        public Int32 Timeout { get; set; }

        /// <summary>
        /// Gets/Sets the encyrption mode for the SSL connection.
        /// </summary>
        /// 
        public FtpEncryptionMode EncryptionMode { get; set; }

        /// <summary>
        /// Gets/Sets the data connection type.
        /// </summary>
        /// 
        public FtpDataConnectionType DataConnectionType { get; set; }

        /// <summary>
        /// Initialize socket level keepalive.
        /// </summary>
        /// 
        public Boolean SocketKeepAlive { get; set; }

        /// <summary>
        /// Set the data transfer type for upload/downloads.
        /// </summary>
        /// 
        public FtpDataType DataTransferType { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Client(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.logFactory = logFactory;

            this.Port = 0;
            this.Timeout = 60;
            this.Secure = false;
            this.SSLcacert = "";
            this.OverWrite = false;
            this.VerifyPeer = false;
            this.Host = "localhost";
            this.Username = "anonymous";
            this.SocketKeepAlive = false;
            this.Password = "kevin@kesteb.us";
            this.CreateRemoteDirectory = false;
            this.DataTransferType = FtpDataType.Binary;
            this.EncryptionMode = FtpEncryptionMode.None;
            this.DataConnectionType = FtpDataConnectionType.AutoPassive;

            client = new FtpClient();
            log = logFactory.Create(typeof(Client));

        }

        /// <summary>
        /// Connect to the host.
        /// </summary>
        /// <returns>True on success.</returns>
        /// 
        public Boolean Connect() {

            bool stat = false;

            client.Host = this.Host;
            client.ConnectTimeout = this.Timeout * 1000;
            client.EnableThreadSafeDataConnections = true;
            client.SocketKeepAlive = this.SocketKeepAlive;
            client.UploadDataType = this.DataTransferType;
            client.DownloadDataType = this.DataTransferType;
            client.DataConnectionType = this.DataConnectionType;
            client.Credentials = new NetworkCredential(this.Username, this.Password);

            if (this.Secure) {

                client.DataConnectionEncryption = true;
                client.EncryptionMode = this.EncryptionMode;
                client.ValidateCertificate += new FtpSslValidation(onValidateCertificate);
                client.SslProtocols = (SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12);

                if (!String.IsNullOrEmpty(this.SSLcacert)) {

                    X509Certificate2 cacert = new X509Certificate2(this.SSLcacert);
                    client.ClientCertificates.Add(cacert);

                }

            } else {

                client.Port = this.Port;

            }

            if (log.IsDebugEnabled) {

                FtpTrace.LogIP = true;
                FtpTrace.LogUserName = true;
                FtpTrace.FlushOnWrite = true;

                FtpTrace.AddListener(new FtpTracer(config, logFactory) {
                    Filter = new EventTypeFilter(SourceLevels.Verbose | SourceLevels.ActivityTracing)
                });

            }

            client.Connect();

            if (client.IsConnected) {

                stat = true;

            }

            return stat;

        }

        /// <summary>
        /// Disconnect from the host.
        /// </summary>
        /// 
        public void Disconnect() {

            client.Disconnect();

        }

        /// <summary>
        /// Get a file from the host.
        /// </summary>
        /// <param name="source">The souce path.</param>
        /// <param name="destination">The destination path.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="cancelSource">A CancellationTokenSource object.</param>
        /// <returns>True if successfull.</returns>
        /// 
        public Boolean Get(String source, String destination, String filename, CancellationTokenSource cancelSource) {

            log.Trace("Entering Get()");

            bool stat = false;
            Stream outStream = null;
            Stream downStream = null;
            string local = Path.Combine(destination, filename);
            string remote = Path.Combine(source, filename).Replace("\\", "/");

            log.Debug(String.Format("FTP.Get() - local: {0}, remote: {1}", local, remote));

            Int64 offset = 0;
            Int32 readBytes = 0;
            Int64 fileLength = 0;
            byte[] buffer = new byte[client.TransferChunkSize];

            if (this.DataTransferType == FtpDataType.Binary) {

                fileLength = client.GetFileSize(remote);

            }

            bool readToEnd = (fileLength <= 0);

            log.Debug(String.Format("File size for {0} is {1}", remote, fileLength));

            using (outStream = new FileStream(local, FileMode.Create, FileAccess.Write, FileShare.None)) {

                using (downStream = client.OpenRead(remote, this.DataTransferType, 0, fileLength > 0)) {

                    while ((offset < fileLength) || readToEnd) {

                        try {

                            while ((readBytes = downStream.Read(buffer, 0, buffer.Length)) > 0) {

                                // check for a cancellation. 

                                if (cancelSource.Token.IsCancellationRequested) {

                                    goto fini;

                                }

                                outStream.Write(buffer, 0, readBytes);
                                offset += readBytes;

                            }

                            log.Debug(String.Format("offset: {0}", offset));

                            if (readToEnd || (offset == fileLength)) {

                                break;

                            }

                            throw new IOException(
                                String.Format("Unexpected EOF for remote file {0} [{1}/{2} bytes read]", remote, offset, fileLength)
                            );

                        } catch (IOException ex) {

                            if (ResumeDownload(fileLength, offset, ex)) {

                                downStream.Flush();
                                downStream.Dispose();
                                downStream = client.OpenRead(remote, this.DataTransferType, restart: offset);

                            } else {

                                throw;

                            }

                        }

                    }

                    fini:  stat = true;

                }

            }

            log.Trace("Leaving Get()");

            return stat;

        }

        /// <summary>
        /// Get a file from the host.
        /// </summary>
        /// <param name="source">The souce path.</param>
        /// <param name="destination">The destination path.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="token">A CancellationToken object.</param>
        /// <returns>True if successfull.</returns>
        /// 
        public async Task<Boolean> GetAsync(String source, String destination, String filename, CancellationTokenSource cancelSource) {

            log.Trace("Entering GetAsync()");

            bool stat = await Task.Run(() => Get(source, destination, filename, cancelSource), cancelSource.Token);

            log.Trace("Leaving GetAysnc()");

            return stat;

        }

        /// <summary>
        /// Put a file on the ftp server.
        /// </summary>
        /// <param name="source">The source path.</param>
        /// <param name="destination">The destination path.</param>
        /// <param name="filename">The file name</param>
        /// <param name="cancelSource">A CancellationTokenSource object.</param>
        /// <returns>True if successfull.</returns>
        /// 
        public Boolean Put(String source, String destination, String filename, CancellationTokenSource cancelSource) {

            log.Trace("Entering Put()");

            long offset = 0;
            bool stat = false;
            Stream outStream = null;
            Stream fileStream = null;
            bool fileExists = false;
            string local = Path.Combine(source, filename);
            string remote = Path.Combine(destination, filename).Replace("\\", "/");
            FtpExists existsMode = (OverWrite) ? FtpExists.Overwrite : FtpExists.NoCheck;

            log.Debug(String.Format("FTP.Put - local: {0}, remote: {1}", local, remote));

            // check if the file exists, and skip, overwrite or append

            if (existsMode != FtpExists.NoCheck) {

                fileExists = client.FileExists(remote);

                switch (existsMode) {
                    case FtpExists.Skip:
                        if (fileExists) {

                            log.Warn("File " + remote + " exists on server & existsMode is set to FileExists.Skip");
                            return false;

                        }
                        break;

                    case FtpExists.Overwrite:
                        if (fileExists) {

                            client.DeleteFile(remote);

                        }
                        break;

                    case FtpExists.Append:
                        if (fileExists) {

                            offset = client.GetFileSize(remote);

                            if (offset == -1) {

                                offset = 0; // start from the beginning

                            }

                        }
                        break;
                }

            }

            // ensure the remote dir exists .. only if the file does not already exist!

            if (this.CreateRemoteDirectory && !fileExists) {

                string dirname = remote.GetFtpDirectoryName();

                if (!client.DirectoryExists(dirname)) {

                    client.CreateDirectory(dirname);

                }

            }

            // open stream connections

            if (offset == 0) {

                outStream = client.OpenWrite(remote, this.DataTransferType);

            } else {

                outStream = client.OpenAppend(remote, this.DataTransferType);

            }

            fileStream = new FileStream(local, FileMode.Open, FileAccess.Read, FileShare.None);

            if (fileStream.CanSeek) {

                try {

                    // seek to required offset

                    fileStream.Position = offset;

                } catch { }

            }

            // loop till entire file uploaded

            long len = fileStream.Length;
            byte[] buffer = new byte[client.TransferChunkSize];

            try {

                while (offset < len) {

                    try {

                        // read a chunk of bytes from the file

                        int readBytes;

                        while ((readBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0) {

                            // check for a cancellation request

                            if (cancelSource.Token.IsCancellationRequested) {

                                goto fini;

                            }

                            // write chunk to the FTP stream

                            outStream.Write(buffer, 0, readBytes);
                            outStream.Flush();

                            offset += readBytes;

                        }

                        // zero return value (with no Exception) indicates EOS; so we should terminate the outer loop here

                        break;

                    } catch (IOException ex) {

                        // resume if server disconnected midway, or throw if there is an exception doing that as well

                        if (ResumeUpload(fileStream.Length, offset, ex)) {

                            outStream.Dispose();
                            outStream = client.OpenAppend(remote, this.DataTransferType, true);
                            outStream.Position = offset;

                        } else {

                            throw;

                        }

                    }

                }

                fini:  stat = true;

            } finally {

                outStream.Dispose();
                fileStream.Dispose();

            }

            log.Trace("Leaving Put()");

            return stat;

        }

        /// <summary>
        /// Put a file on the ftp server.
        /// </summary>
        /// <param name="source">The source path.</param>
        /// <param name="destination">The destination path.</param>
        /// <param name="filename">The file name</param>
        /// <param name="cancelSource">A CancellationTokenSource object.</param>
        /// <returns>True is successful.</returns>
        /// 
        public async Task<Boolean> PutAsync(String source, String destination, String filename, CancellationTokenSource cancelSource) {

            log.Trace("Entering PutAsync()");

            bool stat = await Task.Run(() => Put(source, destination, filename, cancelSource), cancelSource.Token);

            log.Trace("Leaving PutAsync()");

            return stat;

        }

        /// <summary>
        /// List the files in directoy.
        /// </summary>
        /// <param name="directory">The wanted directory.</param>
        /// <returns>An array of file names.</returns>
        /// 
        public String[] List(String directory) {

            List<string> listing = new List<string>();

            client.SetWorkingDirectory(directory);

            foreach (string file in client.GetNameListing()) {

                listing.Add(file);

            }

            return listing.ToArray();

        }

        /// <summary>
        /// Get file info.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <param name="dt">A reference to a DateTime object.</param>
        /// <param name="size">A reference to a Int32.</param>
        /// 
        public void Info(String filename, out DateTime dt, out Int32 size) {

            size = (Int32)client.GetFileSize(filename);
            dt = client.GetModifiedTime(filename);

        }

        /// <summary>
        /// Checks if a file exists on the ftp server.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>True if the file exists.</returns>
        /// 
        public Boolean Exists(String filename) {

            bool stat = false;
            // FtpListOption flag = (FtpListOption.ForceList | FtpListOption.AllFiles);

            if (client.FileExists(filename)) {

                stat = true;

            }

            return stat;

        }

        /// <summary>
        /// Rename a file on the FTP server.
        /// </summary>
        /// <param name="oldFilename">The old filename.</param>
        /// <param name="newFilename">The new filename.</param>
        /// <returns>True for success.</returns>
        /// 
        public Boolean Rename(String oldFilename, string newFilename) {

            bool stat = false;

            if (client.FileExists(oldFilename)) {

                client.Rename(oldFilename, newFilename);
                stat = true;

            }

            return stat;

        }

        /// <summary>
        /// Delete a file from the FTP server.
        /// </summary>
        /// <param name="filename">The filename to delete.</param>
        /// <returns>True on success.</returns>
        /// 
        public Boolean Remove(String filename) {

            bool stat = false;

            if (client.FileExists(filename)) {

                client.DeleteFile(filename);
                stat = true;

            }

            return stat;

        }

        /// <summary>
        /// Check to see if a directory exists onn the FTP server.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>True on success.</returns>
        /// 
        public Boolean DirectoryExists(String path) {

            return client.DirectoryExists(path);

        }

        /// <summary>
        /// Create a directory on the FTP server.
        /// </summary>
        /// <param name="directory">The directory name.</param>
        /// <returns>True on success.</returns>
        /// 
        public Boolean MakeDirectory(String directory) {

            bool stat = false;

            if (!client.DirectoryExists(directory)) {

                client.CreateDirectory(directory);
                stat = true;

            }

            return stat;

        }

        /// <summary>
        /// Delete a directory from the FTP server.
        /// </summary>
        /// <param name="directory">The directory name.</param>
        /// <returns>True for success.</returns>
        /// 
        public Boolean RemoveDirectory(String directory) {

            bool stat = false;

            if (client.DirectoryExists(directory)) {

                client.DeleteDirectory(directory);
                stat = true;

            }

            return stat;

        }

        #region Private Methods

        private void onValidateCertificate(FtpClient control, FtpSslValidationEventArgs e) {

            if (e.PolicyErrors != System.Net.Security.SslPolicyErrors.None) {

                // invalid cert, do you want to accept it?

                log.Debug(String.Format("onValidateSertificate - invalid certificate - accept {0}", VerifyPeer));

                e.Accept = this.VerifyPeer;

            }

        }

        private Boolean ResumeDownload(long fileSize, long offset, IOException ex) {

            // Possible socket errors
            //
            //  ANSI C         Winsock           C#
            // --------------+-----------------+--------+-------------------------------------
            // EPIPE         | ??              | ??     | Broken pipe.
            // ETIMEDOUT     | WSAETIMEDOUT    | 10060  | Connection timed out.
            // ECONNRESET    | WSAECONNRESET   | 10054  | Connection reset by peer
            // ECONNREFUSED  | WSAECONNREFUSED | 10061  | Connection refused.
            // ECONNABORTED  | WSAECONNABORTED | 10053  | Software caused connection abort.
            // ENETUNREACH   | WSAENETUNREACH  | 10051  | Network is unreachable.
            // ENETDOWN      | WSAENETDOWN     | 10050  | Network is down.
            // ENETRESET     | WSAENETRESET    | 10052  | Network dropped connection on reset.
            // EWOULDBLOCK   | WSAEWOULDBLOCK  | 10035  | Resource temporarily unavailable.

            if (ex.InnerException != null) {

                var ie = ex.InnerException as System.Net.Sockets.SocketException;

                log.Debug(String.Format("Message: {0}, ErrorCode: {1}", ie.Message, ie.ErrorCode));

                if (ie != null && ie.ErrorCode == 10054) {

                    return true;

                }

            } else {

                if (ex.Message.Contains("Unexpected EOF")) {

                    log.Warn(ex.Message);

                    if (offset < fileSize) {

                        return true;

                    }

                }

            }

            return false;

        }


        private bool ResumeUpload(long fileSize, long offset, IOException ex) {

            if (ex.InnerException != null) {

                var ie = ex.InnerException as System.Net.Sockets.SocketException;

                log.Debug(String.Format("Message: {0}, ErrorCode: {1}", ie.Message, ie.ErrorCode));

                if (ie != null && ie.ErrorCode == 10054) {

                    return true;

                }

            } else  {

                if (offset < fileSize) {

                    return true;

                }

            }

            return false;
        }

        #endregion
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Generic dispose.
        /// </summary>
        /// <param name="disposing"></param>

        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    // TODO: dispose managed state (managed objects).

                    this.client.Dispose();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

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
        #region Internal TraceListener

        class FtpTracer: TraceListener {

            private readonly ILogger log = null;
            private readonly IConfiguration config = null;

            public FtpTracer(IConfiguration config, ILoggerFactory logFactory) {

                this.config = config;

                log = logFactory.Create(typeof(FtpTracer));

            }
            
            public override void Write(string message) {

                log.Debug(String.Format("{0}", message).Trim());

            }

            public override void WriteLine(string message) {

                log.Debug(String.Format("{0}", message).Trim());

            }

        }

        #endregion

    }

}

