using System;
using System.Threading;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace ServiceSupervisor.Supervisor {

    public class Handler {

        private readonly ILogger log = null;
        private readonly IManager manager = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly CancellationTokenSource cancelSource = null;

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IHandler object.</param>
        /// <param name="logFactory">A ILoggerFactory object.</param>
        /// <param name="manager">An IManager object.</param>
        /// <param name="cancelSource">A CancellationTokenSource object.</param>
        /// 
        public Handler(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IManager manager, CancellationTokenSource cancelSource) {

            var key = config.Key;
            var section = config.Section;

            this.config = config;
            this.handler = handler;
            this.manager = manager;
            this.cancelSource = cancelSource;

            this.log = logFactory.Create(typeof(Handler));
 
        }

        public void Process() {



        }

    }

}
