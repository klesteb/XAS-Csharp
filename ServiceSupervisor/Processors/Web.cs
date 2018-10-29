using System;

using XAS.Rest.Server;
using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

using ServiceSupervisor.Configuration.Extensions;

namespace ServiceSupervisor.Processors {

    /// <summary>
    /// Processor for REST requests.
    /// </summary>
    /// 
    public class Web {

        private readonly ILogger log = null;
        private readonly Server server = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Web(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            var key = config.Key;
            var section = config.Section;

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Web));

            // get config stuff
            
            Uri uri = new Uri(config.GetValue(section.Web(), key.Address()));
            string rootPath = config.GetValue(section.Web(), key.WebRootPath());
            string domain = config.GetValue(section.Environment(), key.Domain());
            bool enableClientCertificates = config.GetValue(section.Web(), key.EnableClientCertificates()).ToBoolean();

            // build the bootstrapper

            var authenticate = new Authenticate();
            var userValidator = new UserValidator(authenticate, domain);
            var appRootProvider = new AppRootPathProvider { RootPath = rootPath };
            var bootStrapper = new ServiceSupervisor.Web.BootStrapper(config, handler, logFactory, userValidator, appRootProvider);

            // launch the server

            server = new Server(config, handler, logFactory, uri, domain, rootPath, enableClientCertificates, bootStrapper);

        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");
            server.Start();
            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");
            server.Stop();
            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause the server.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");
            server.Pause();
            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Continue the server.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue()");
            server.Continue();
            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Perform shutdown activities.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");
            server.Shutdown();
            log.Trace("Leaving Shutdown()");

        }

    }

}
