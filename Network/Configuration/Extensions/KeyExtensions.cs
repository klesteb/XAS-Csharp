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

    }

}
