using System;

using XAS.Core.Configuration;

namespace XAS.Network.Configuration.Extensions {

    /// <summary>
    /// Configuration Key extensions for Network.
    /// </summary>
    /// 
    public static class KeyExtensions {

        public static String ConnectionTimeoutException(this Key junk) {
            return "ConnectionTimeoutException";
        }

        public static String StompAckException(this Key junk) {
            return "StompAckException";
        }

        public static String StompNackVersionException(this Key junk) {
            return "StompNackVersionException";
        }

        public static String StompSubscribeException(this Key junk) {
            return "StompSubscribeException";
        }

        public static String StompNackSubscrptionExecption(this Key junk) {
            return "StompNackSubscrptionExecption";
        }

        public static String StompUnsubscribeException(this Key junk) {
            return "StompUnsubscribeException";
        }

        public static String ClientConnect(this Key junk) {
            return "ClientConnect";
        }

        public static String ClientInactive(this Key junk) {
            return "ClientInactive";
        }

        public static String ClientDeadSocket(this Key junk) {
            return "ClientDeadSocket";
        }

        public static String ClientProblems(this Key junk) {
            return "ClientProblems";
        }

        public static String ClientSSLValidation(this Key junk) {
            return "ClientSSLValidation";
        }

        public static String ServerDisconnect(this Key junk) {
            return "ServerDisconnect";
        }

        public static String ServerConnect(this Key junk) {
            return "ServerConnect";
        }

        public static String ServerReconnect(this Key junk) {
            return "ServerReconnect";
        }

        public static String TcpKeepalive(this Key junk) {
            return "TcpKeepalive";
        }

    }

}
