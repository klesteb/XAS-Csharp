using System;
using System.Threading;
using System.Threading.Tasks;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Processes;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using ServiceSupervisor.Model.Services;
using XAS.Core;

namespace ServiceSupervisor.Processors {

    /// <summary>
    /// Processor for the supervisor.
    /// </summary>
    /// 
    public class Supervisor {

        private bool stopProcessing = false;

        private readonly ILogger log = null;
        private readonly IManager manager = null;
        private readonly Supervised service = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly ILoggerFactory logFactory = null;

        private CancellationTokenSource cancelSource = null;

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
            this.logFactory = logFactory;
            this.log = logFactory.Create(typeof(Supervisor));
            this.service = new Supervised(config, handler, logFactory);

        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            stopProcessing = false;
            cancelSource = new CancellationTokenSource();

            using (var repo = manager.Repository as Model.Repositories) {

                var jobs = service.List(repo);

                foreach (var job in jobs) {

                    job.Spawn = new Spawn(config, handler, logFactory, job.Config);
                    
                    job.Spawn.OnExit += ExitHandler;
                    job.Spawn.OnStderr += StderrHandler;
                    job.Spawn.OnStdout += StdoutHandler;
                    job.Spawn.OnStarted += StartedHandler;

                    job.Spawn.Start();
                    job.Status = Model.Schema.RunStatus.Started;

                    service.Update(repo, job.Name, job);

                }

            }

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            stopProcessing = true;
            cancelSource.Cancel();

            using (var repo = manager.Repository as Model.Repositories) {

                var jobs = service.List(repo);

                foreach (var job in jobs) {

                    if (job.Status == Model.Schema.RunStatus.Running) {

                        job.Spawn.Dispose();
                        job.Status = Model.Schema.RunStatus.Stopped;

                        service.Update(repo, job.Name, job);

                    }

                }

            }

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
        /// Continue the server.
        /// </summary>
        /// 
        public void Continue() {

            log.Trace("Entering Continue()");

            Start();

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Perform shutdown activities.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            Stop();

            log.Trace("Leaving Shutdown()");

        }

        #region Private Methods

        private void ExitHandler(Int32 pid, Int32 exitCode) {

            log.Trace("Entering ExitHandler()");

            using (var repo = manager.Repository as Model.Repositories) {

                var job = service.Get(repo, pid);

                if (job != null) {

                    if (! stopProcessing && 
                        job.Config.AutoRestart && 
                        (job.Config.ExitCodes.Contains(exitCode)) &&
                        (job.RetryCount <= job.Config.ExitRetries)) {

                        Utils.Sleep(job.Config.RestartDelay, cancelSource.Token);

                        job.Spawn.Start();
                        job.RetryCount++;

                    } else {

                        job.Pid = 0;
                        job.Status = Model.Schema.RunStatus.Stopped;

                        log.WarnMsg("", "");

                    }

                    service.Update(repo, job.Name, job);

                }

            }

            log.Trace("Leaving ExitHandler()");

        }

        private void StartedHandler(Int32 pid, String name) {

            log.Trace("Entering StderrHandler()");

            using (var repo = manager.Repository as Model.Repositories) {

                var job = service.Get(repo, name);

                if (job != null) {

                    if (job.Spawn.Stat()) {

                        job.Status = Model.Schema.RunStatus.Running;
                        service.Update(repo, job.Name, job);

                        log.InfoMsg("", job.Name);

                    }

                }

            }

            log.Trace("Leaving StderrHandler()");

        }

        private void StderrHandler(Int32 pid, String buffer) {

            log.Trace("Entering StderrHandler()");

            using (var repo = manager.Repository as Model.Repositories) {

                var job = service.Get(repo, pid);

                if (job != null) {

                    log.ErrorMsg("", job.Name, buffer);

                }

            }

            log.Trace("Leaving StderrHandler()");

        }

        private void StdoutHandler(Int32 pid, String buffer) {

            log.Trace("Entering StdoutHandler()");

            using (var repo = manager.Repository as Model.Repositories) {

                var job = service.Get(repo, pid);

                if (job != null) {

                    log.InfoMsg("", job.Name, buffer);

                }

            }

            log.Trace("Leaving StdoutHandler()");

        }

        #endregion

    }

}
