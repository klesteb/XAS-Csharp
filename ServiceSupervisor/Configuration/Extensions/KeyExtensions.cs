using System;

using XAS.Core.Configuration;

namespace ServiceSupervisor.Configuration.Extensions {

    /// <summary>
    /// Key extension for the ServiceSupervisor.
    /// </summary>
    /// 
    public static class KeyExtensions {

        // messages

        public static String GET3(this Key junk) {
            return "GET3";
        }

        public static String PUT4(this Key junk) {
            return "PUT4";
        }

        public static String OPTIONS3(this Key junk) {
            return "OPTIONS3";
        }

        public static String OPTIONS4(this Key junk) {
            return "OPTIONS4";
        }

        public static String PUT_NoStart(this Key junk) {
            return "PUT_NoStart";
        }

        public static String PUT_NoStop(this Key junk) {
            return "PUT_NoStop";
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

        public static String SupervisorExitRetries(this Key junk) {
            return "ExitRetries";
        }

        public static String SupervisorAutoRestart(this Key junk) {
            return "AutoRestart";
        }

        public static String SupervisorRestartDelay(this Key junk) {
            return "RestartDelay";
        }

        public static String SupervisorStopDelay(this Key junk) {
            return "StopDelay";
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
