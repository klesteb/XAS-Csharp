using System;

using Nancy.Hosting.Self;

using XAS.Core;
using XAS.Rest.Server;
using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;

using DemoMicroServiceServer.Configuration;

namespace DemoMicroServiceServer.Processors {

    /// <summary>
    /// Processor for REST requests.
    /// </summary>
    /// 
    public class Web {

        private readonly ILogger log = null;
        private readonly Server server = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        public Web(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Web));

            var key = config.Key;
            var section = config.Section;

            Uri uri = new Uri(config.GetValue(section.Web(), key.Address()));
            string domain = config.GetValue(section.Environment(), key.Domain());
            string rootPath = config.GetValue(section.Web(), key.WebRootPath());
            bool enableClientCertificates = config.GetValue(section.Web(), key.EnableClientCertificates()).ToBoolean();

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

            var authenticate = new Authenticate();
            var userValidator = new UserValidator(authenticate, domain);
            var appRootProvider = new AppRootPathProvider { RootPath = rootPath };
            var bootStrapper = new BootStrapper(config, handler, logFactory, userValidator, appRootProvider, null);

            server = new Server(logFactory, bootStrapper, hostConfig, uri);

        }

        public void Start() {

            log.Trace("Entering Start()");
            server.Start();
            log.Trace("Leaving Start()");

        }

        public void Stop() {

            log.Trace("Entering Stop()");
            server.Stop();
            log.Trace("Leaving Stop()");

        }

        public void Pause() {

            log.Trace("Entering Pause()");
            server.Pause();
            log.Trace("Leaving Pause()");

        }

        public void Continue() {

            log.Trace("Entering Continue()");
            server.Continue();
            log.Trace("Leaving Continue()");

        }

        public void Shutdown() {

            log.Trace("Entering Shutdown()");
            server.Shutdown();
            log.Trace("Leaving Shutdown()");

        }

    }

}
