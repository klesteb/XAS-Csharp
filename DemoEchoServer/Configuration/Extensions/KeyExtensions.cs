using System;

using XAS.Core.Configuration;

namespace DemoEchoServer.Configuration.Extensions {

    /// <summary>
    /// Key extension for the DemoMicroServiceServer.
    /// </summary>
    /// 
    public static class KeyExtensions {

        // messages

        // config file

        public static String Address(this Key junk) {
            return "Address";
        }

        public static String Port(this Key junk) {
            return "Port";
        }

        public static String Backlog(this Key junk) {
            return "Backlog";
        }

        public static String MaxConnections(this Key junk) {
            return "MaxConnections";
        }

        public static String ClientTimeout(this Key junk) {
            return "ClientTimeout";
        }

        public static String ReaperInterval(this Key junk) {
            return "ReaperInterval";
        }

        public static String UseSSL(this Key junk) {
            return "ServerUseSSL";
        }

        public static String SSLCaCert(this Key junk) {
            return "ServerSSLCaCert";
        }

        public static String SSLVerifyPeer(this Key junk) {
            return "ServerSSLVerifyPeer";
        }

        public static String SSLProtocols(this Key junk) {
            return "ServerSSLProtocols";
        }

    }

}
