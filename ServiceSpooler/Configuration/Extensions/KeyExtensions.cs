using System;

using XAS.Core.Configuration;

namespace ServiceSpooler.Configuration.Extensions {

    /// <summary>
    /// Configuration Key extensions for ServiceSpooler.
    /// </summary>
    /// 
    public static class KeyExtensions {

        // messages

        public static String CorruptFile(this Key junk) {
            return "CorruptFile";
        }

        public static String WatchDirectory(this Key junk) {
            return "WatchDirectory";
        }

        public static String NoDirectory(this Key junk) {
            return "NoDirectory";
        }

        public static String FileFound(this Key junk) {
            return "FileFound";
        }

        public static String UnknownFile(this Key junk) {
            return "UnknowFile";
        }

        public static String UnlinkFile(this Key junk) {
            return "UnlinkFile";
        }

        public static String Disconnected(this Key junk) {
            return "Disconnected";
        }

        public static String Connected(this Key junk) {
            return "Connected";
        }

        public static String ProtocolError(this Key junk) {
            return "ProtocolError";
        }

        // configs

        public static String Server(this Key junk) {
            return "server";
        }

        public static String Port(this Key junk) {
            return "port";
        }

        public static String UseSSL(this Key junk) {
            return "use-ssl";
        }

        public static String Password(this Key junk) {
            return "password";
        }

        public static String KeepAlive(this Key junk) {
            return "keepalive";
        }

        public static String Level(this Key junk) {
            return "level";
        }

        public static String Queue(this Key junk) {
            return "queue";
        }

        public static String PacketType(this Key junk) {
            return "packet-type";
        }

        public static String Alias(this Key junk) {
            return "alias";
        }

    }

}
