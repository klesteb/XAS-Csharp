using System;

using XAS.Core.Configuration;

namespace ServiceSupervisor.Configuration.Extensions {

    /// <summary>
    /// Key extension for the DemoMicroServiceServer.
    /// </summary>
    /// 
    public static class KeyExtensions {

        // messages

        public static String GET(this Key junk) {
            return "Processing GET(/{0}) for {1}";
        }

        public static String GETS(this Key junk) {
            return "Processing GET(/{0}/{1}) for {2}";
        }

        public static String OPTION(this Key junk) {
            return "Processing OPTIONS(/{0}/{1}) for {2}";
        }

        public static String OPTIONS(this Key junk) {
            return "Processing OPTIONS(/{0}) for {1}";
        }

        // config file - web

        public static String Address(this Key junk) {
            return "Address";
        }

        public static String EnableClientCertificates(this Key junk) {
            return "EnableClientCertificates";
        }

        public static String WebRootPath(this Key junk) {
            return "WebRootPath";
        }

        // config file - supervisor

        public static String SupervisorCommand(this Key junk) {
            return "Command";
        }

        public static String SupervisorUsername(this Key junk) {
            return "Username";
        }

        public static String SupervisorPassword(this Key junk) {
            return "Password";
        }

        public static String SupervisorVerb(this Key junk) {
            return "Verb";
        }

        public static String SupervisorAutoStart(this Key junk) {
            return "AutoStart";
        }

        public static String SupervisorExitRetries(this Key junk) {
            return "ExitRetries";
        }

        public static String SupervisorAutoRestart(this Key junk) {
            return "AutoRestart";
        }

        public static String SupervisorRestartDelay(this Key junk) {
            return "RestartDelay";
        }

        public static String SupervisorExitCodes(this Key junk) {
            return "ExitCodes";
        }

        public static String SupervisorWorkingDirectory(this Key junk) {
            return "WorkingDirectory";
        }

        public static String SupervisorEnvironment(this Key junk) {
            return "Environment";
        }

        public static String SupervisorDomain(this Key junk) {
            return "Domain";
        }

    }

}
