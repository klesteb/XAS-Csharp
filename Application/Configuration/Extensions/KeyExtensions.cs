using System;

using XAS.Core.Configuration;

namespace XAS.App.Configuration.Extensions {

    /// <summary>
    /// Configuration Key extensions for Application.
    /// </summary>
    /// 
    public static class KeyExtensions {

        public static String InvalidOption(this Key junk) {
            return "InvalidOption";
        }

        public static String UnknownOptions(this Key junk) {
            return "UnknownOptions";
        } 

        public static String InvalidOperation(this Key junk) {
            return "InvalidOperation";
        }

        public static String InvalidOptionsNoValue(this Key junk) {
            return "InvalidOptionsNoValue";
        }

        public static String InvalidOptionsUnknowOption(this Key junk) {
            return "InvalidOptionsUnknowOption";
        }

        public static String ProcessInterrupt(this Key junk) {
            return "ProcessInterrupt";
        }

        public static String ServicePaused(this Key junk) {
            return "ServicePaused";
        }

        public static String ServiceResumed(this Key junk) {
            return "ServiceResumed";
        }

        public static String ServiceStopped(this Key junk) {
            return "ServiceStopped";
        }

        public static String ServiceStartup(this Key junk) {
            return "ServiceStartup";
        }

        public static String ServiceCustom(this Key junk) {
            return "ServiceCustom";
        }

        public static String ServiceShutdown(this Key junk) {
            return "ServiceShutdown";
        }

        public static String IWindowsServiceType(this Key junk) {
            return "IWindowsServiceType";
        }

        public static String ServiceImplementation(this Key junk) {
            return "ServiceImplementation";
        }

        public static String WindowsServiceAttribute(this Key junk) {
            return "WindowsServiceAttribute";
        }

        public static String IWindowsServiceImplementation(this Key junk) {
            return "IWindowsServiceImplementation";
        }

        public static String NoLogConf(this Key junk) {
            return "NoLogConf";
        }

        public static String InvLogType(this Key junk) {
            return "InvLogType";
        }

        public static String UnknownCommand(this Key junk) {
            return "UnknownCommand";
        }

    }

}
