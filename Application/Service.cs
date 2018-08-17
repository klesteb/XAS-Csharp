using System;
using System.ServiceProcess;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.App.Services.Framework;

namespace XAS.App {

    /// <summary>
    /// Base class for Windows Services.
    /// </summary>
    /// 
    public class Service: Base {

        private readonly ILogger log = null;

        /// <summary>
        /// Get/Sets the IWindowsService handle.
        /// </summary>
        /// 
        public IWindowsService WindowsService { get; set; }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// 
        public Service(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity security): 
            base(config, handler, logFactory, security) {

            this.log = logFactory.Create(typeof(Service));

        }

        /// <summary>
        /// Entry point for a service application. 
        /// </summary>
        /// <remarks>
        /// This method sets up the environment for a console application. It establishes a global exception 
        /// handler. A way to process command line arugments and lastly returns a meaniful return code to the 
        /// Service Control Manager.
        /// </remarks>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Return code for the SCM.</returns>
        /// 
        public Int32 Run(string[] args) {

            int rc = 0;

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
        /// Main entry point for a service. 
        /// </summary>
        /// <remarks>
        /// If the service is ran interactively. The program will run as a console based application. This can
        /// help with debugging the service.
        /// </remarks>
        /// <param name="args">Command line arguments.</param>
        /// <returns>A return code for the shell or SCM.</returns>
        /// 
        public override Int32 RunApp(String[] args) {

            Int32 rc = 0;

            if (Environment.UserInteractive) {

                System.Console.TreatControlCAsInput = false;
                System.Console.CancelKeyPress += new ConsoleCancelEventHandler(OnConsoleKeyPress);

                // if started from console, file explorer, etc, run as console app.

                ConsoleHarness.Run(args, this.WindowsService);

            } else {

                // otherwise run as a windows service

                ServiceBase.Run(new WindowsServiceHarness(this.WindowsService));

            }

            return rc;

        }

        /// <summary>
        /// An override of GetOptions(). It provides additonal options for installing and deinstalling services.
        /// </summary>
        /// <remarks>
        /// In order to install a service, you need Administrator privileges. This method will detect if they
        /// are activated. If not, it will rerun the program in "Administrator" context so that it can be installed.
        /// </remarks>
        /// <returns>An Options object.</returns>
        /// 
        public override Options GetOptions() {

            Options options = base.GetOptions();

            options.Add("install|i", "install the service", (v) => {

                if (security.IsElevated) {

                    WindowsServiceInstaller.RuntimeInstall(this.WindowsService);

                } else {

                    security.RunElevated();

                }

                Environment.Exit(0);

            });

            options.Add("uninstall|u", "uninstall the service", (v) => {

                if (security.IsElevated) {

                    WindowsServiceInstaller.RuntimeUnInstall(this.WindowsService);

                } else {

                    security.RunElevated();

                }

                Environment.Exit(0);

            });

            return options;

        }

        #endregion

    }

}
