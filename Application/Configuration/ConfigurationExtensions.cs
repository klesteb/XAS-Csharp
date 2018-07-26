
using System.IO;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Loaders;

namespace XAS.App.Configuration {

    public static class ConfigurationExtensions {

        /// <summary>
        /// Populate the Messages section of the configuration, with Application specific messages.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public static void LoadApplicationMessages(this IConfiguration config) {

            var key = config.Key;
            var section = config.Section;

            config.AddKey(
                section.Messages(),
                key.InvalidOperation(),
                "Invalid operation"
            );

            config.AddKey(
                section.Messages(),
                key.InvalidOption(),
                "Invalid option: {0}"
            );

            config.AddKey(
                section.Messages(),
                key.InvalidOptionsNoValue(),
                "Expecting a value after option \"{0}\", found \"{1}\""
            );

            config.AddKey(
                section.Messages(),
                key.InvalidOptionsUnknowOption(),
                "Unsupported using bundled options \"{0}\" that requires a value"
            );

            config.AddKey(
                section.Messages(),
                key.ServiceStartup(),
                "Service has been started"
            );

            config.AddKey(
                section.Messages(),
                key.ServicePaused(),
                "Service has been paused"
            );

            config.AddKey(
                section.Messages(),
                key.ServiceResumed(),
                "Service has been resumed"
            );

            config.AddKey(
                section.Messages(),
                key.ServiceStopped(),
                "Service has been stopped"
            );

            config.AddKey(
                section.Messages(),
                key.ServiceShutdown(),
                "Service has been shutdown"
            );

            config.AddKey(
                section.Messages(),
                key.ServiceImplementation(),
                "IWindowsService cannot be null in call to GenericWindowsService"
            );

            config.AddKey(
                section.Messages(),
                key.IWindowsServiceImplementation(),
                "IWindowsService implementer {0} must have a WindowsServiceAttribute"
            );

            config.AddKey(
                section.Messages(),
                key.IWindowsServiceType(),
                "Type to install must implement IWindowsService: \"{0}\""
            );

            config.AddKey(
                section.Messages(),
                key.WindowsServiceAttribute(),
                "Type to install must be marked with a WindowsServiceAttribute: \"{0}\""
            );

            config.AddKey(
                section.Messages(),
                key.ProcessInterrupt(),
                "Process interrupted by signal {0}"
            );

            config.AddKey(
                section.Messages(),
                key.NoLogConf(),
                "The configuration file: \"{0}\", doesn't exist."
            );

            config.AddKey(
                section.Messages(),
                key.InvLogType(),
                "Invalid log type: {0}, should be \"console\", \"file\", \"json\" or \"event\"."
            );

            config.AddKey(
                section.Messages(),
                key.UnknownCommand(),
                "I didn't understand that command."
            );

        }

    }

}
