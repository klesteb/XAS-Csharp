using System;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace ServiceSupervisor.Processors {

    /// <summary>
    /// Processor for the supervisor.
    /// </summary>
    /// 
    public class Supervisor {

        private readonly ILogger log = null;
        private readonly ISupervisor server = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        protected IManager manager = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Supervisor(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IManager manager) {

            var key = config.Key;
            var section = config.Section;

            this.config = config;
            this.handler = handler;
            this.manager = manager;

            this.log = logFactory.Create(typeof(Supervisor));

            // get config stuff
            
            // launch the server

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
