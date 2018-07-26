using System;
using System.ServiceProcess;

using XAS.Core.Configuration;

namespace XAS.App.Services.Framework {

    // taken from: http://geekswithblogs.net/BlackRabbitCoder/archive/2011/03/01/c-toolbox-debug-able-self-installable-windows-service-template-redux.aspx
    // with modifications

    /// <summary>
    /// A generic Windows Service that can handle any assembly that
    /// implements IWindowsService (including AbstractWindowsService) 
    /// </summary>
    /// 
    public partial class WindowsServiceHarness: ServiceBase {

        /// <summary>
        /// Get the class implementing the windows service
        /// </summary>
        /// 
        public IWindowsService ServiceImplementation { get; private set; }

        /// <summary>
        /// Constructor a generic windows service from the given class
        /// </summary>
        /// <param name="serviceImplementation">Service implementation.</param>
        /// 
        public WindowsServiceHarness(IWindowsService serviceImplementation) {

            // make sure service passed in is valid

            if (serviceImplementation == null) {

                throw new ArgumentNullException("IWindowsService cannot be null in call to GenericWindowsService");

            }

            // set instance and backward instance

            ServiceImplementation = serviceImplementation;

            // configure our service

            ConfigureServiceFromAttributes(serviceImplementation);

        }

        /// <summary>
        /// Override service control on continue
        /// </summary>
        /// 
        protected override void OnContinue() {

            ServiceImplementation.OnContinue();

        }

        /// <summary>
        /// Called when service is paused
        /// </summary>
        /// 
        protected override void OnPause() {

            ServiceImplementation.OnPause();

        }

        /// <summary>
        /// Called when a custom command is requested
        /// </summary>
        /// <param name="command">Id of custom command</param>
        /// 
        protected override void OnCustomCommand(int command) {

            ServiceImplementation.OnCustomCommand(command);

        }

        /// <summary>
        /// Called when the Operating System is shutting down
        /// </summary>
        /// 
        protected override void OnShutdown() {

            ServiceImplementation.OnShutdown();

        }

        /// <summary>
        /// Called when service is requested to start
        /// </summary>
        /// <param name="args">The startup arguments array.</param>
        /// 
        protected override void OnStart(string[] args) {

            ServiceImplementation.OnStart(args);

        }

        /// <summary>
        /// Called when service is requested to stop
        /// </summary>
        /// 
        protected override void OnStop() {

            ServiceImplementation.OnStop();

        }

        /// <summary>
        /// Set configuration data
        /// </summary>
        /// <param name="serviceImplementation">The service with configuration settings.</param>
        /// 
        private void ConfigureServiceFromAttributes(IWindowsService serviceImplementation) {

            var attribute = serviceImplementation.GetType().GetAttribute<WindowsServiceAttribute>();

            if (attribute != null) {

                // wire up the event log source, if provided

                if (!string.IsNullOrWhiteSpace(attribute.EventSource)) {

                    // assign to the base service's EventLog property for auto-log events.

                    EventLog.Log = attribute.EventLog;
                    EventLog.Source = attribute.EventSource;
                
                }

                this.AutoLog = attribute.AutoLog;
                this.CanStop = attribute.CanStop;
                this.CanShutdown = attribute.CanShutdown;
                this.ServiceName = attribute.ServiceName;
                this.CanPauseAndContinue = attribute.CanPauseAndContinue;

                // we don't handle: laptop power change event

                this.CanHandlePowerEvent = false;

                // we don't handle: Term Services session event

                this.CanHandleSessionChangeEvent = false;

            } else {

                throw new InvalidOperationException(
                    String.Format("IWindowsService implementer {0} must have a WindowsServiceAttribute", serviceImplementation.GetType().FullName)
                );

            }

        }

    }

}
