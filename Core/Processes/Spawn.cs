using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace XAS.Core.Processes {

    public delegate Int32 StartHandler(Int32 pid, String name);
    public delegate void ExitHandler(Int32 pid, Int32 exitCode);
    public delegate void StdoutHandler(Int32 pid, String buffer);
    public delegate void StderrHandler(Int32 pid, String buffer);

    /// <summary>
    /// Spawn a process and keep it running.
    /// </summary>
    /// 
    public class Spawn: IDisposable {

        private Int32 retries = 0;
        private Int32 exitCode = 0;

        private readonly ILogger log = null;
        private readonly ISecurity secure = null;
        private readonly SpawnInfo spawnInfo = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly ProcessStartInfo startInfo = null;
        private readonly System.Diagnostics.Process process = null;

        /// <summary>
        /// Set the event handler to handle an exit event.
        /// </summary>
        /// 
        public event ExitHandler OnExit;

        /// <summary>
        /// Set the envent handler to handle output on stderr.
        /// </summary>
        /// 
        public event StderrHandler OnStderr;

        /// <summary>
        /// Set the event handler to handle output on stdout.
        /// </summary>
        /// 
        public event StdoutHandler OnStdout;

        /// <summary>
        /// Set the event handler to handle the process startup.
        /// </summary>
        /// 
        public event StartHandler OnStarted;

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// <param name="spawnInfo">A SpawnInfo object.</param>
        /// 
        public Spawn(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, SpawnInfo spawnInfo) {
 
            this.config = config;
            this.handler = handler;
            this.spawnInfo = spawnInfo;
            this.secure = new Secure();
            this.log = logFactory.Create(typeof(Spawn));

            // parse the command line

            string[] args = Utils.ParseCommandLine(spawnInfo.Command);
            int length = args.Length - 1;

            // set up the process

            this.process = new System.Diagnostics.Process();
            this.startInfo = new ProcessStartInfo() {
                FileName = args[0],
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = spawnInfo.WorkingDirectory,
                Arguments = String.Join(" ", args.Skip(1).Take(length).ToArray()),
            };

            // set user context, if any

            if ((! String.IsNullOrEmpty(spawnInfo.Username) && 
                (! String.IsNullOrEmpty(spawnInfo.Password)))) {

                startInfo.LoadUserProfile = true;
                startInfo.Domain = spawnInfo.Domain;
                startInfo.UserName = spawnInfo.Username;
                startInfo.Password = secure.MakeSecureString(spawnInfo.Password);

            }

            // set the verb context

            if (! String.IsNullOrEmpty(spawnInfo.Verb)) {

                startInfo.Verb = spawnInfo.Verb;

            }

            // add environment variables, if any

            foreach (KeyValuePair<String, String> env in spawnInfo.Environment) {

                // replace existing environment variables

                if (startInfo.Environment.ContainsKey(env.Key)) {

                    startInfo.Environment[env.Key] = env.Value;
                    continue;

                }

                startInfo.Environment.Add(env.Key, env.Value);

            }

            process.EnableRaisingEvents = true;
            process.StartInfo = startInfo;
            process.Exited += ExitHandler;
            process.ErrorDataReceived += StderrHandler;
            process.OutputDataReceived += StdoutHandler;

        }

        /// <summary>
        /// Start a process.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            retries = 0;
            exitCode = 0;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (OnStarted != null) {

                OnStarted(process.Id, spawnInfo.Name);

            }

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop a process.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            process.CloseMainWindow();

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Stat a process.
        /// </summary>
        /// <returns>true if the process still exists.</returns>
        /// 
        public Boolean Stat() {

            log.Trace("Entering Stat()");

            bool stat = false;

            try {
            
                var junk = System.Diagnostics.Process.GetProcessById(process.Id);
                stat = true;

            } catch { }

            log.Trace("Leaving Stat()");

            return stat;

        }

        /// <summary>
        /// Pause a process.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");
            log.Trace("Leaving Pause()");
        }

        /// <summary>
        /// Resume a process.
        /// </summary>
        /// 
        public void Resume() {

            log.Trace("Entering Resume()");
            log.Trace("Leaving Resume()");

        }

        /// <summary>
        /// Kill a process.
        /// </summary>
        /// 
        public void Kill() {

            log.Trace("Entering Kill()");

            process.Kill();

            log.Trace("Leaving Kill()");

        }

        /// <summary>
        /// Return the processes exit code.
        /// </summary>
        /// <returns>A integer value.</returns>
        /// 
        public Int32 ExitCode() {

            return exitCode;

        }

        #region Private Methods

        private void RestartHandler() {

            log.Trace("Entering RestartHandler()");

            // restart logic

            retries++;

            if (! spawnInfo.ExitCodes.Contains(exitCode)) {

                if ((spawnInfo.AutoRestart) && (retries <= spawnInfo.ExitRetries)) {

                    if (spawnInfo.RestartDelay > 0) {

                        Thread.Sleep(spawnInfo.RestartDelay * 1000);

                    }

                    Start();

                }

            }

            log.Trace("Leaving RestartHandler()");

        }

        private void ExitHandler(object sender, EventArgs e) {

            log.Trace("Entering ExitHandler()");

            Int32 id = process.Id;
            exitCode = process.ExitCode;

            // do some cleanup

            process.CancelOutputRead();
            process.CancelErrorRead();
            process.Close();

            // call the exit handler callback

            if (OnExit != null) {

                RestartHandler();

            }

            log.Trace("Leaving ExitHandler()");

        }

        private void StdoutHandler(object sender, DataReceivedEventArgs e) {

            log.Trace("Entering StdoutHandler()");

            if (! String.IsNullOrEmpty(e.Data)) {

                string buffer = e.Data.Trim();

                if (OnStdout != null) {

                    OnStdout(process.Id, buffer);

                } else {

                    log.Info(buffer);

                }

            }

            log.Trace("Leaving StdoutHandler()");

        }

        private void StderrHandler(object sender, DataReceivedEventArgs e) {

            log.Trace("Entering StderrHandler()");

            if (! String.IsNullOrEmpty(e.Data)) {

                string buffer = e.Data.Trim();

                if (OnStderr != null) {

                    OnStderr(process.Id, buffer);

                } else {

                    log.Error(buffer);

                }

            }

            log.Trace("Leaving StderrHandler()");

        }

        #endregion
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    // stop the process.

                    int count = 0;
                    Stop();

                    while (Stat()) {

                        count++;

                        if (count > 5) {

                            Kill();
                            break;

                        }

                        Thread.Sleep(5 * 1000);

                    }

                    // release the handlers

                    foreach (ExitHandler item in OnExit.GetInvocationList()) {

                        OnExit -= item;

                    }

                    foreach (StderrHandler item in OnStderr.GetInvocationList()) {

                        OnStderr -= item;

                    }

                    foreach (StdoutHandler item in OnStdout.GetInvocationList()) {

                        OnStdout -= item;

                    }

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;

            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Spawn() {
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
