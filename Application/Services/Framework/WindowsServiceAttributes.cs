using System;
using System.ServiceProcess;

namespace XAS.App.Services.Framework {

    // taken from: http://geekswithblogs.net/BlackRabbitCoder/archive/2011/03/01/c-toolbox-debug-able-self-installable-windows-service-template-redux.aspx
    // with modifications

    /// <summary>
    /// Attributes that can be applied to a service.
    /// </summary>
    ///
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WindowsServiceAttribute: Attribute {

        /// <summary>
        /// The name of the service.
        /// </summary>
        ///
        public string ServiceName { get; set; }

        /// <summary>
        /// The displayable name that shows in service manager (defaults to Name).
        /// </summary>
        ///
        public string DisplayName { get; set; }

        /// <summary>
        /// A textural description of the service name (defaults to Name).
        /// </summary>
        ///
        public string Description { get; set; }

        /// <summary>
        /// The user to run the service under (defaults to null).  A null or empty
        /// UserName field causes the service to run as ServiceAccount.LocalService.
        /// </summary>
        ///
        public string Username { get; set; }

        /// <summary>
        /// The password to run the service under (defaults to null).  Ignored
        /// if the UserName is empty or null, this property is ignored.
        /// </summary>
        ///
        public string Password { get; set; }

        /// <summary>
        /// Specifies the event log source to set the service's EventLog to.  If this is
        /// empty or null (the default) no event log source is set.  If set, will auto-log
        /// start and stop events.
        /// </summary>
        /// 
        public string EventSource { get; set; }

        /// <summary>
        /// Specifies the event log to send entries too. Defaults to "Application".
        /// </summary>
        /// 
        public String EventLog { get; set; }

        /// <summary>
        /// The method to start the service when the machine reboots (defaults to Manual).
        /// </summary>
        /// 
        public ServiceStartMode StartMode { get; set; }

        /// <summary>
        /// True if service supports pause and continue (defaults to true).
        /// </summary>
        /// 
        public bool CanPauseAndContinue { get; set; }

        /// <summary>
        /// True if service supports shutdown event (defaults to true).
        /// </summary>
        /// 
        public bool CanShutdown { get; set; }

        /// <summary>
        /// True if service supports stop event (defaults to true).
        /// </summary>
        /// 
        public bool CanStop { get; set; }

        /// <summary>
        /// True if service is suppposed to auto log to the Event log (defaults to false).
        /// </summary>
        /// 
        public Boolean AutoLog { get; set; }

        /// <summary>
        /// Marks an IWindowsService with configuration and installation attributes.
        /// </summary>
        /// <param name="name">The name of the windows service.</param>
        /// 
        public WindowsServiceAttribute(string name) {

            // set name and default description and display name to name.

            ServiceName = name;
            Description = name;
            DisplayName = name;

            // default all other attributes.

            CanStop = true;
            Password = null;
            Username = null;
            AutoLog = false;
            CanShutdown = true;
            EventLog = "Application";
            EventSource = ServiceName;
            CanPauseAndContinue = true;
            StartMode = ServiceStartMode.Manual;

        }

    }

}