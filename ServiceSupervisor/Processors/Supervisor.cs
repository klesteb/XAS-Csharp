using System;
using System.Threading;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using ServiceSupervisor.Supervisor;
using System.Threading.Tasks;

namespace ServiceSupervisor.Processors {

    /// <summary>
    /// Processor for the supervisor.
    /// </summary>
    /// 
    public class Supervisor {

        private readonly ILogger log = null;
        private readonly Handler server = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly CancellationTokenSource cancelSource = null;

        private Task task = null;

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
            this.cancelSource = new CancellationTokenSource();

            this.log = logFactory.Create(typeof(Supervisor));

            // get config stuff

            // launch the server

            this.server = new Handler(config, handler, logFactory, manager, cancelSource);

        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            task = new Task(server.Process, cancelSource.Token, TaskCreationOptions.LongRunning);
            task.Start();

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        /// 
        public void Stop() {

            Task[] tasks = { task };

            log.Trace("Entering Stop()");

            cancelSource.Cancel();
            Task.WaitAll(tasks);

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause the server.
        /// </summary>
        /// 
        public void Pause() {

            Task[] tasks = { task };

            log.Trace("Entering Pause()");

            cancelSource.Cancel();
            Task.WaitAll(tasks);

            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Continue the server.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue()");

            task = new Task(server.Process, cancelSource.Token, TaskCreationOptions.LongRunning);
            task.Start();

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Perform shutdown activities.
        /// </summary>
        /// 
        public void Shutdown() {

            Task[] tasks = { task };

            log.Trace("Entering Shutdown()");

            cancelSource.Cancel();
            Task.WaitAll(tasks);

            log.Trace("Leaving Shutdown()");

        }

    }

}
