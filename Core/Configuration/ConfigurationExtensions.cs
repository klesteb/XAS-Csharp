using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

using XAS.Core.Locking;
using XAS.Core.Configuration.Messages;

namespace XAS.Core.Configuration {
    
    /// <summary>
    /// Extension class for configuration.
    /// </summary>
    /// 
    public static class ConfigurationExtensions {

        /// <summary>
        /// Build the basic configuration.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public static void Build(this IConfiguration config) {

            var key = config.Key;
            var section = config.Section;

            string[] _args = System.Environment.GetCommandLineArgs();
            string script = Path.GetFileName(_args[0]);
            string dnsDomain = System.Environment.GetEnvironmentVariable("USERDNSDOMAIN").ToLower();

            config.CreateSection(section.Application());
            config.CreateSection(section.Environment());
            config.CreateSection(section.Messages());

            config.AddKey(section.Environment(), key.DnsDomain(),
                ((String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_DNSDOMAIN")))
                    ? dnsDomain
                    : System.Environment.GetEnvironmentVariable("XAS_DNSDOMAIN")).ToLower()
            );

            config.AddKey(section.Environment(), key.Domain(),
                ((String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_DOMAIN")))
                    ? System.Environment.GetEnvironmentVariable("USERDOMAIN")
                    : System.Environment.GetEnvironmentVariable("XAS_DOMAIN")).ToUpper()
            );

            config.AddKey(section.Environment(), key.Host(),
                (System.Environment.OSVersion.Platform == PlatformID.Unix ||
                 System.Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? System.Environment.GetEnvironmentVariable("HOSTNAME")
                    : ((String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("COMPUTERNAME")))
                        ? System.Environment.MachineName
                        : System.Environment.GetEnvironmentVariable("COMPUTERNAME")).ToLower()
            );

            config.AddKey(section.Environment(), key.Username(),
                (System.Environment.OSVersion.Platform == PlatformID.Unix ||
                 System.Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? System.Environment.GetEnvironmentVariable("USER")
                    : System.Environment.GetEnvironmentVariable("USERNAME")
            );

            config.AddKey(section.Environment(), key.HomeDir(),
                (System.Environment.OSVersion.Platform == PlatformID.Unix ||
                 System.Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? System.Environment.GetEnvironmentVariable("HOME")
                    : Path.Combine(
                        System.Environment.GetEnvironmentVariable("HOMEDRIVE"),
                        System.Environment.GetEnvironmentVariable("HOMEPATH")
                      )
            );

            config.AddKey(section.Environment(), key.MQServer(),
                ((String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_MQSERVER")))
                    ? "mq." + dnsDomain
                    : System.Environment.GetEnvironmentVariable("XAS_MQSERVER").ToLower()).ToLower()
            );

            config.AddKey(section.Environment(), key.MQPort(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_MQPORT")))
                    ? "61613"
                    : System.Environment.GetEnvironmentVariable("XAS_MQPORT")
            );

            config.AddKey(section.Environment(), key.MXServer(),
                ((String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_MXSERVER")))
                    ? "mail." + dnsDomain
                    : System.Environment.GetEnvironmentVariable("XAS_MXSERVER")).ToLower()
            );

            config.AddKey(section.Environment(), key.MXPort(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_MXPORT")))
                    ? "25"
                    : System.Environment.GetEnvironmentVariable("XAS_MXPORT")
            );

            config.AddKey(section.Environment(), key.MXTimeout(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_MXTIMEOUT")))
                    ? "60"
                    : System.Environment.GetEnvironmentVariable("XAS_MXTIMEOUT")
            );

            config.AddKey(section.Environment(), key.MXMailer(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_MAILER")))
                    ? "smtp"
                    : System.Environment.GetEnvironmentVariable("XAS_MAILER")
            );

            config.AddKey(section.Environment(), key.RootDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_ROOT")))
                    ? "C:\\XAS"
                    : System.Environment.GetEnvironmentVariable("XAS_ROOT")
            );

            string root = config.GetValue(section.Environment(), "RootDir");

            config.AddKey(section.Environment(), key.EtcDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_ETC")))
                    ? Path.Combine(root, "etc")
                    : System.Environment.GetEnvironmentVariable("XAS_ETC")
            );

            config.AddKey(section.Environment(), key.SbinDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_SBIN")))
                    ? Path.Combine(root, "sbin")
                    : System.Environment.GetEnvironmentVariable("XAS_SBIN")
            );

            config.AddKey(section.Environment(), key.TempDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_TMP")))
                    ? Path.Combine(root, "tmp")
                    : System.Environment.GetEnvironmentVariable("XAS_TMP")
            );

            config.AddKey(section.Environment(), key.BinDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_BIN")))
                    ? Path.Combine(root, "bin")
                    : System.Environment.GetEnvironmentVariable("XAS_BIN")
            );

            config.AddKey(section.Environment(), key.LibDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_LIBS")))
                    ? Path.Combine(root, "lib")
                    : System.Environment.GetEnvironmentVariable("XAS_LIBS")
            );

            config.AddKey(section.Environment(), key.VarLibDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_LIB")))
                    ? Path.Combine(root, "var", "lib")
                    : System.Environment.GetEnvironmentVariable("XAS_LIB")
            );

            config.AddKey(section.Environment(), key.VarLockDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_LOCKS")))
                    ? Path.Combine(root, "var", "locks")
                    : System.Environment.GetEnvironmentVariable("XAS_LOCKS")
            );

            config.AddKey(section.Environment(), key.VarRunDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_RUN")))
                    ? Path.Combine(root, "var", "run")
                    : System.Environment.GetEnvironmentVariable("XAS_RUN")
            );

            config.AddKey(section.Environment(), key.VarSpoolDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_SPOOL")))
                    ? Path.Combine(root, "var", "spool")
                    : System.Environment.GetEnvironmentVariable("XAS_SPOOL")
            );

            config.AddKey(section.Environment(), key.VarLogDir(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_LOG")))
                    ? Path.Combine(root, "var", "log")
                    : System.Environment.GetEnvironmentVariable("XAS_LOG")
            );

            config.AddKey(section.Environment(), key.LogType(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_LOGTYPE"))
                    ? "console"
                    : System.Environment.GetEnvironmentVariable("XAS_LOGTYPE"))
            );

            config.AddKey(section.Environment(), key.LogLevel(),
                (String.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("XAS_LOGLEVEL"))
                    ? "info"
                    : System.Environment.GetEnvironmentVariable("XAS_LOGLEVEL"))
            );

            // application stuff

            config.AddKey(section.Environment(), key.CommandLine(), System.Environment.CommandLine);
            config.AddKey(section.Environment(), key.Script(), Path.GetFileName(_args[0]));
            config.AddKey(section.Environment(), key.Priority(), "low");
            config.AddKey(section.Environment(), key.Facility(), "systems");
            config.AddKey(section.Environment(), key.Pid(), Process.GetCurrentProcess().Id.ToString());

            config.AddKey(section.Environment(), key.LogFile(),
                 String.Format("{0}\\{1}.log",
                    config.GetValue(section.Environment(), key.VarLogDir()),
                    Path.GetFileNameWithoutExtension(script))
            );

            config.AddKey(section.Environment(), key.LogConf(),
                String.Format("{0}\\{1}.conf",
                    config.GetValue(section.Environment(), key.EtcDir()),
                    Path.GetFileNameWithoutExtension(script))
            );

            config.AddKey(section.Environment(), key.PidFile(),
                String.Format("{0}\\{1}.pid",
                    config.GetValue(section.Environment(), key.VarRunDir()),
                    Path.GetFileNameWithoutExtension(script))
            );

            config.AddKey(section.Environment(), key.CfgFile(),
                String.Format("{0}\\{1}.ini",
                    config.GetValue(section.Environment(), key.EtcDir()),
                    Path.GetFileNameWithoutExtension(script))
            );

            config.AddKey(section.Environment(), key.LockName(), "locked");
            config.AddKey(section.Environment(), key.Alerts(), true.ToString());
            config.AddKey(section.Environment(), key.Debug(), false.ToString());
            config.AddKey(section.Environment(), key.Trace(), false.ToString());
            config.AddKey(section.Environment(), key.LockDriver(), LockDriver.Mutex.ToString());

            // find and load messages
            // taken from https://stackoverflow.com/questions/5120647/instantiate-all-classes-implementing-a-specific-interface
            // with modifications

            var interfaceType = typeof(IMessages);
            var loaders = AppDomain.CurrentDomain.GetAssemblies()
              .SelectMany(x => x.GetTypes())
              .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .Select(x => Activator.CreateInstance(x));

            foreach (var loader in loaders) {

                var messages = loader as IMessages;
                messages.Load(config);

            }

        }

    }

}
