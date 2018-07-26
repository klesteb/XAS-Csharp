using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;


namespace XAS.App.Services.Framework {

    // taken from: http://geekswithblogs.net/BlackRabbitCoder/archive/2011/03/01/c-toolbox-debug-able-self-installable-windows-service-template-redux.aspx
    // with modifications

    /// <summary>
    /// A generic windows service installer
    /// </summary>
    /// 
    [RunInstaller(true)]
    public partial class WindowsServiceInstaller: Installer {

        /// <summary>
        /// Gets or sets the type of the windows service to install.
        /// </summary>
        /// 
        public WindowsServiceAttribute Configuration { get; set; }

        /// <summary>
        /// Creates a blank windows service installer with configuration in ServiceImplementation
        /// </summary>
        /// 
        public WindowsServiceInstaller() : this(typeof(ServiceImplementation)) { }

        /// <summary>
        /// Creates a windows service installer using the type specified.
        /// </summary>
        /// <param name="windowsServiceType">The type of the windows service to install.</param>
        /// 
        public WindowsServiceInstaller(Type windowsServiceType) {

            InitializeComponent();

            if (!windowsServiceType.GetInterfaces().Contains(typeof(IWindowsService))) {

                throw new ArgumentException(
                    String.Format("Type to install must implement IWindowsService: \"{0}\"", "windowsServiceType")
                );

            }

            var attribute = windowsServiceType.GetAttribute<WindowsServiceAttribute>();

            if (attribute == null) {

                throw new ArgumentException(
                    String.Format("Type to install must be marked with a WindowsServiceAttribute: \"{0}\"", "windowsServiceType")
                );

            }

            Configuration = attribute;

        }

        /// <summary>
        /// Performs a transacted installation at run-time of the AutoCounterInstaller and any other listed installers.
        /// </summary>
        /// <param name="service">An IWindowsService object.</param>
        /// 
        public static void RuntimeInstall(IWindowsService service) {

            Type type = service.GetType();
            string[] args = { "/assemblypath=" + Assembly.GetEntryAssembly().Location };
            WindowsServiceInstaller installer = new WindowsServiceInstaller(type);

            using (var ti = new TransactedInstaller()) {

                ti.Installers.Add(installer);
                ti.Context = new InstallContext(installer.Configuration.ServiceName + ".install.log", args);
                ti.Install(new Hashtable());

            }

        }

        /// <summary>
        /// Performs a transacted un-installation at run-time of the AutoCounterInstaller and any other listed installers.
        /// </summary>
        /// <param name="service">An IWindowsService object. </param>
        /// 
        public static void RuntimeUnInstall(IWindowsService service) {

            Type type = service.GetType();
            string[] args = { "/assemblypath=" + Assembly.GetEntryAssembly().Location };
            WindowsServiceInstaller installer = new WindowsServiceInstaller(type);

            using (var ti = new TransactedInstaller()) {

                ti.Installers.Add(installer);
                ti.Context = new InstallContext(installer.Configuration.ServiceName + ".install.log", args);
                ti.Uninstall(null);

            }

        }

        /// <summary>
        /// Check to see if the service is running, and stop it.
        /// </summary>
        /// <param name="savedState"></param>
        /// 
        protected override void OnBeforeInstall(IDictionary savedState) {

            try {

                ServiceController controller = new ServiceController(Configuration.ServiceName);

                if ((controller.Status == ServiceControllerStatus.Running) || (controller.Status ==  ServiceControllerStatus.Paused)) {

                    controller.Stop();

                    controller.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 15));

                    controller.Close();

                }

            } catch {

                // not installed yet, ignore the exception

            }

            base.OnBeforeInstall(savedState);

        }

        /// <summary>
        /// Install the service.
        /// </summary>
        /// <param name="savedState">The saved state for the installation.</param>
        /// 
        public override void Install(IDictionary savedState) {

            // install the service 

            ConfigureInstallers();
            base.Install(savedState);

            // wire up the event log source, if provided

            if (!string.IsNullOrWhiteSpace(Configuration.EventSource)) {

                // create the source if it doesn't exist

                if (!EventLog.SourceExists(Configuration.EventSource)) {

                    EventLog.CreateEventSource(Configuration.EventSource, Configuration.EventLog);

                }

            }

        }

        /// <summary>
        /// Check to see if the service is running, and stop it.
        /// </summary>
        /// <param name="savedState"></param>
        /// 
        protected override void OnBeforeUninstall(IDictionary savedState) {

            ServiceController controller = new ServiceController(Configuration.ServiceName);

            if ((controller.Status == ServiceControllerStatus.Running) || (controller.Status == ServiceControllerStatus.Paused)) {

                controller.Stop();

                controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 15));

                controller.Close();

            }

            base.OnBeforeUninstall(savedState);

        }

        /// <summary>
        /// Uninstall the service.
        /// </summary>
        /// <param name="savedState">The saved state for the installation.</param>
        /// 
        public override void Uninstall(IDictionary savedState) {

            // load the assembly file name and the config

            ConfigureInstallers();
            base.Uninstall(savedState);

            // wire up the event log source, if provided

            if (!string.IsNullOrWhiteSpace(Configuration.EventSource)) {

                // delete the source if it exists

                if (EventLog.SourceExists(Configuration.EventSource)) {

                    EventLog.DeleteEventSource(Configuration.EventSource);

                }

            }

        }

        /// <summary>
        /// Rolls back to the state of the counter, and performs the normal rollback.
        /// </summary>
        /// <param name="savedState">The saved state for the installation.</param>
        /// 
        public override void Rollback(IDictionary savedState) {

            // load the assembly file name and the config

            ConfigureInstallers();
            base.Rollback(savedState);

        }

        /// <summary>
        /// Method to configure the installers.
        /// </summary>
        /// 
        private void ConfigureInstallers() {

            // load the assembly file name and the config

            Installers.Add(ConfigureProcessInstaller());
            Installers.Add(ConfigureServiceInstaller());

        }

        /// <summary>
        /// Helper method to configure a process installer for this windows service
        /// </summary>
        /// <returns>Process installer for this service</returns>
        /// 
        private ServiceProcessInstaller ConfigureProcessInstaller() {

            var result = new ServiceProcessInstaller();

            // if a user name is not provided, will run under local service acct

            if (string.IsNullOrEmpty(Configuration.Username)) {

                result.Account = ServiceAccount.LocalService;
                result.Username = null;
                result.Password = null;

            } else {

                // otherwise, runs under the specified user authority

                result.Account = ServiceAccount.User;
                result.Username = Configuration.Username;
                result.Password = Configuration.Password;

            }
         
            result.AfterInstall += new InstallEventHandler(ServiceProcessInstaller_AfterInstall);

            return result;

        }

        /// <summary>
        /// Helper method to configure a service installer for this windows service
        /// </summary>
        /// <returns>Process installer for this service</returns>
        /// 
        private ServiceInstaller ConfigureServiceInstaller() {

            // create and config a service installer

            var result = new ServiceInstaller {
                ServiceName = Configuration.ServiceName,
                DisplayName = Configuration.DisplayName,
                Description = Configuration.Description,
                StartType = Configuration.StartMode,
            };

            result.AfterInstall += new InstallEventHandler(ServiceInstaller_AfterInstall);

            return result;

        }

        private void ServiceInstaller_AfterInstall(Object sender, InstallEventArgs e) {

            // do nothing

        }

        private void ServiceProcessInstaller_AfterInstall(Object sender, InstallEventArgs e) {

            // do nothing

        }

    }

}
