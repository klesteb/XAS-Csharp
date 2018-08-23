using System;

using Nancy.Hosting.Self;
using Nancy.Bootstrapper;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Rest.Server.Errors.Exceptions;

namespace XAS.Rest.Server {

    /// <summary>
    /// A REST based server.
    /// </summary>
    /// 
    public class Server: IDisposable {

        private readonly ILogger log = null;
        private readonly NancyHost host = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// <param name="bootStrapper">A NancyBootstrapper object. </param>
        /// <param name="hostConfig">A HostConfiguration object. </param>
        /// <param name="uri">An Url object.</param>
        /// 
        public Server(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, Uri uri, String domain, String rootPath, Boolean enableClientCertificates, INancyBootstrapper bootStrapper) {

            this.log = logFactory.Create(typeof(Server));

            var hostConfig = new HostConfiguration {
                UrlReservations = new UrlReservations {
                    CreateAutomatically = true,
                    User = "Everyone"
                },
                RewriteLocalhost = true,
                AllowChunkedEncoding = false,
                EnableClientCertificates = enableClientCertificates,
                UnhandledExceptionCallback = e => {
                    handler.Exceptions(e);
                }
            };

            // initialize nancy

            if ((host = new NancyHost(bootStrapper, hostConfig, uri)) == null) {

                throw new InternalServerErrorException("Unable to initialize Nancy");

            }

        }

        /// <summary>
        /// Start processing requests.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start().");
            host.Start();
            log.Trace("Leaving start().");

        }

        /// <summary>
        /// Stop processing requests.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop().");
            host.Dispose();
            log.Trace("Leaving Stop().");

        }

        /// <summary>
        /// Pause request processing.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause().");
            host.Dispose();
            log.Trace("Leaving Pause().");

        }

        /// <summary>
        /// Continue request processing.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue().");
            host.Start();
            log.Trace("Leaving Continue().");

        }

        /// <summary>
        /// Perform shutdown actions.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown().");
            host.Dispose();
            log.Trace("Leaving Shutdown().");

        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {
                    // TODO: dispose managed state (managed objects).

                    host.Dispose();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RestHost() {
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
