using System;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace XAS.App {

    /// <summary>
    /// Base class for console applications.
    /// </summary>
    ///
    public class Console: Base {

        private ILogger log = null;

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// 
        public Console(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity security): 
            base(config, handler, logFactory, security) {

            log = logFactory.Create(typeof(Console));

        }

        /// <summary>
        /// Entry point for a console application. 
        /// </summary>
        /// <remarks>
        /// This method sets up the environment for a console application. It establishes a global exception 
        /// handler. A ^C or ^BREAK handler, a way to process command line arugments and lastly returns
        /// a meaniful return code to the shell, which places it into the %ERRORLEVEL% environment variable so that
        /// other programs can determine if the program ran correctly.
        /// </remarks>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Return code for the shell.</returns>
        /// 
        public Int32 Run(string[] args) {

            int rc = 0;

            System.Console.TreatControlCAsInput = false;
            System.Console.CancelKeyPress += new ConsoleCancelEventHandler(OnConsoleKeyPress);

            try {

                string[] parameters = this.ProcessOptions(args);

                rc = this.RunApp(parameters);

            } catch (Exception ex) {

                rc = handler.Exit(ex);

            }

            return rc;

        }

        /// <summary>
        /// An override of GetOptions(). It provides an additonal option for minimizing the console window. 
        /// Usefull for when console applications are run under the Task Scheduler.
        /// </summary>
        /// <returns>An Options object.</returns>
        /// 
        public override Options GetOptions() {

            Options options = base.GetOptions();

            options.Add("minwin", "minimizes the console window", (v) => {
                IntPtr winHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                ShowWindow(winHandle, SW_SHOWMINIMIZED);
            });

            return options;
    
        }

    }

}
