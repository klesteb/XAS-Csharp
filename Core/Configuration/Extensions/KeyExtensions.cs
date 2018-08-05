using System;

namespace XAS.Core.Configuration.Extensions {

    /// <summary>
    /// Key extensions.
    /// </summary>
    public static class KeyExtensions {

        // messages

        public static String StartRun(this Key junk) {
            return "startrun";
        }

        public static String StopRun(this Key junk) {
            return "stoprun";
        }
        public static String StartUp(this Key junk) {
            return "startup";
        }

        public static String ShutDown(this Key junk) {
            return "shutdown";
        }

        public static String ArgumentIsNull(this Key junk) {
            return "ArgumentIsNull";
        }

        public static String InvalidMimeType(this Key junk) {
            return "InvalidMimeType";
        }

        public static String MimeTypeNotRegistered(this Key junk) {
            return "MimeTypeNotRegistered";
        }

        public static String FileMissing(this Key junk) {
            return "FileMissing";
        }

        // environment

        public static String Domain(this Key junk) {
            return "Domain";
        }

        public static String DnsDomain(this Key junk) {
            return "DnsDomain";
        }

        public static String Host(this Key junk) {
            return "Host";
        }

        public static String Username(this Key junk) {
            return "Username";
        }

        public static String HomeDir(this Key junk) {
            return "HomeDir";
        }

        public static String MQServer(this Key junk) {
            return "MQServer";
        }

        public static String MQPort(this Key junk) {
            return "MQPort";
        }

        public static String MXServer(this Key junk) {
            return "MXServer";
        }

        public static String MXPort(this Key junk) {
            return "MXPort";
        }

        public static String MXTimeout(this Key junk) {
            return "MXTimeout";
        }

        public static String MXMailer(this Key junk) {
            return "MXMailer";
        }

        public static String RootDir(this Key junk) {
            return "RootDir";
        }

        public static String EtcDir(this Key junk) {
            return "EtcDir";
        }

        public static String SbinDir(this Key junk) {
            return "SbinDir";
        }

        public static String TempDir(this Key junk) {
            return "TempDir";
        }

        public static String BinDir(this Key junk) {
            return "BinDir";
        }

        public static String LibDir(this Key junk) {
            return "LibDir";
        }

        public static String VarLibDir(this Key junk) {
            return "VarLibDir";
        }

        public static String VarLockDir(this Key junk) {
            return "VarLockDir";
        }

        public static String VarRunDir(this Key junk) {
            return "VarRunDir";
        }

        public static String VarSpoolDir(this Key junk) {
            return "VarSpoolDir";
        }

        public static String VarLogDir(this Key junk) {
            return "VarLogDir";
        }

        // application

        public static String LogType(this Key junk) {
            return "LogType";
        }

        public static String LogLevel(this Key junk) {
            return "LogLevel";
        }

        public static String CommandLine(this Key junk) {
            return "CommandLine";
        }

        public static String Script(this Key junk) {
            return "Script";
        }

        public static String Priority(this Key junk) {
            return "Priority";
        }

        public static String Facility(this Key junk) {
            return "Facility";
        }

        public static String Pid(this Key junk) {
            return "Pid";
        }

        public static String LogFile(this Key junk) {
            return "LogFile";
        }

        public static String LogConf(this Key junk) {
            return "LogConf";
        }

        public static String PidFile(this Key junk) {
            return "PidFile";
        }

        public static String CfgFile(this Key junk) {
            return "CfgFile";
        }

        public static String Alerts(this Key junk) {
            return "Alerts";
        }

        public static String Debug(this Key junk) {
            return "Debug";
        }

        public static String Trace(this Key junk) {
            return "Trace";
        }

        public static String LockName(this Key junk) {
            return "LockName";
        }

        public static String LockDriver(this Key junk) {
            return "LockDriver";
        }

    }

}
