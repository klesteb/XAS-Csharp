using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using XAS.Network.TCP;
using XAS.Core.Logging;
using XAS.Core.Extensions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoEchoServer.Configuration.Extensions;

namespace DemoEchoServer.Processors {

    /// <summary>
    /// Implements a basic echo server.
    /// </summary>
    /// 
    public class Echo {

        private Task serverTask = null;
        private CancellationTokenSource cancellationTokenSource = null;

        private readonly ILogger log = null;
        private readonly Server server = null;
        private readonly Decoder decoder = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Echo(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            var key = config.Key;
            var section = config.Section;

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Echo));

            this.decoder = Encoding.UTF8.GetDecoder();
            this.server = new Server(config, handler, logFactory);

            this.server.Port = config.GetValue(section.Server(), key.Port(), "7").ToInt32();
            this.server.Address = config.GetValue(section.Server(), key.Address(), "127.0.0.1");
            this.server.Backlog = config.GetValue(section.Server(), key.Backlog(), "10").ToInt32();
            this.server.ClientTimeout = config.GetValue(section.Server(), key.ClientTimeout(), "0").ToInt32();
            this.server.ReaperInterval = config.GetValue(section.Server(), key.ReaperInterval(), "60").ToInt32();

            this.server.SSLCaCert = config.GetValue(section.SSL(), key.SSLCaCert());
            this.server.UseSSL = config.GetValue(section.SSL(), key.UseSSL(), "false").ToBoolean();
            this.server.SSLVerifyPeer = config.GetValue(section.SSL(), key.SSLVerifyPeer(), "false").ToBoolean();

            this.server.OnServerDataSent += OnDataSent;
            this.server.OnServerDataReceived += OnDataReceived;

        }

        /// <summary>
        /// The handler for when data is received.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="buffer">The received buffer.</param>
        /// 
        public void OnDataReceived(Int32 id, byte[] buffer) {

            int bytes = buffer.Length;
            var client = server.GetClient(id);
            StringBuilder message = new StringBuilder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
            decoder.GetChars(buffer, 0, bytes, chars, 0);
            message.Append(chars);

            log.Info(String.Format("Received data from: {0}, on port: {1}, data: \"{2}\"",
                client.RemoteHost, client.RemotePort, message.ToString()));

            server.Send(id, buffer);

        }

        /// <summary>
        /// The handler for when data is sent.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// 
        public void OnDataSent(Int32 id) {

            var client = server.GetClient(id);

            log.Info(String.Format("Sent data to: {0} on port {1}", client.RemoteHost, client.RemotePort));

        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// 
        public void Start() {

            log.Trace("Entering Start()");

            cancellationTokenSource = new CancellationTokenSource();

            serverTask = new Task(() => {

                server.Cancellation = cancellationTokenSource; 
                server.Start();

            }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning);

            serverTask.Start();

            log.Trace("Leaving Start()");

        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        /// 
        public void Stop() {

            log.Trace("Entering Stop()");

            Task[] tasks = { serverTask };

            cancellationTokenSource.Cancel(true);
            Task.WaitAny(tasks);

            server.Stop();

            log.Trace("Leaving Stop()");

        }

        /// <summary>
        /// Pause the server.
        /// </summary>
        /// 
        public void Pause() {

            log.Trace("Entering Pause()");

            Task[] tasks = { serverTask };

            cancellationTokenSource.Cancel(true);
            Task.WaitAny(tasks);

            server.Pause();

            log.Trace("Leaving Pause()");

        }

        /// <summary>
        /// Continue the server.
        /// </summary>
        /// 
        public void Resume() {

            log.Trace("Entering Continue()");

            cancellationTokenSource = new CancellationTokenSource();

            serverTask = new Task(() => {

                server.Cancellation = cancellationTokenSource;
                server.Resume();

            }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning);

            serverTask.Start();

            log.Trace("Leaving Continue()");

        }

        /// <summary>
        /// Perform shutdown activities.
        /// </summary>
        /// 
        public void Shutdown() {

            log.Trace("Entering Shutdown()");

            Task[] tasks = { serverTask };

            cancellationTokenSource.Cancel(true);
            Task.WaitAny(tasks);

            server.Shutdown();

            log.Trace("Leaving Shutdown()");

        }

    }

}
