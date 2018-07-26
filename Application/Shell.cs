using System;

using XAS.App;
using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace XAS.App {

    /// <summary>
    /// Base class for Windows Services.
    /// </summary>
    ///
    public class Shell: Base {

        private ILogger log = null;

        /// <summary>
        /// Get/Set the command options that will be processed by the shell.
        /// </summary>
        /// 
        public CommandOptions Commands { get; set; }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// 
        public Shell(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity security) : base(config, handler, logFactory, security) {

            log = logFactory.Create(typeof(Shell));

        }

        /// <summary>
        /// Entry point for a shell application. 
        /// </summary>
        /// <remarks>
        /// This method sets up the environment for a console application. It establishes a global exception 
        /// handler. A way to process command line arugments and lastly returns a meaniful return code to the 
        /// Service Control Manager.
        /// </remarks>
        /// <param name="args">Command line arguments.</param>
        /// <returns>A return code.</returns>
        /// 
        public Int32 Run(string[] args) {

            int rc = 0;

            if (! System.Console.IsInputRedirected) {

                System.Console.TreatControlCAsInput = false;
                System.Console.CancelKeyPress += new ConsoleCancelEventHandler(OnConsoleKeyPress);

            }

            try {

                string[] parameters = this.ProcessOptions(args);

                rc = this.RunApp(parameters);

            } catch (Exception ex) {

                rc = handler.Exit(ex);

            }

            return rc;

        }

        #region Overridden Methods

        /// <summary>
        /// Main entry point for a shell. 
        /// </summary>
        /// <remarks>
        /// Run the shell interactively.
        /// </remarks>
        /// <param name="args">Command line arguments.</param>
        /// <returns>A return code for the shell or SCM.</returns>
        /// 
        public override Int32 RunApp(String[] args) {

            Int32 rc = 0;

            rc = Commands.Process(args);

            return rc;

        }

        #endregion

    }

}

