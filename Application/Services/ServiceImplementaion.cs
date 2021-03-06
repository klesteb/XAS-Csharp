﻿using System.ServiceProcess;

using XAS.App.Services.Framework;

namespace XAS.App.Services {

    /// <summary>
    /// The actual implementation of the windows service goes here...
    /// </summary>
    /// 
    [WindowsService("DummyService",
        DisplayName = "DummyService",
        Description = "The description of the Dummyervice service.",
        EventSource = "DummyService",
        StartMode = ServiceStartMode.Manual)]

    public class ServiceImplementation: IWindowsService {

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        /// 
        public void Dispose() { }

        /// <summary>
        /// This method is called when the service gets a request to start.
        /// </summary>
        /// <param name="args">Any command line arguments</param>
        /// 
        public void OnStart(string[] args) { }

        /// <summary>
        /// This method is called when the service gets a request to stop.
        /// </summary>
        /// 
        public void OnStop() { }

        /// <summary>
        /// This method is called when a service gets a request to pause, 
        /// but not stop completely.
        /// </summary>
        /// 
        public void OnPause() { }

        /// <summary>
        /// This method is called when a service gets a request to resume 
        /// after a pause is issued.
        /// </summary>
        /// 
        public void OnContinue() { }

        /// <summary>
        /// This method is called when the machine the service is running on
        /// is being shutdown.
        /// </summary>
        /// 
        public void OnShutdown() { }

        /// <summary>
        /// This method is called when a custom command is issued to the service.
        /// </summary>
        /// <param name="command">The command identifier to execute.</param >
        /// 
        public void OnCustomCommand(int command) { }

    }

}
