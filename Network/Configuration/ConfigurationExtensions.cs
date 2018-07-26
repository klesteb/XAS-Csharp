
using XAS.Core.Configuration;

namespace XAS.Network.Configuration {

    public static class ConfigurationExtensions {

        /// <summary>
        /// Populate the Messages section of the configuration, with Network specific messages.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public static void LoadNetworkMessages(this IConfiguration config) {

            var key = config.Key;
            var section = config.Section;

            config.AddKey(section.Messages(), key.ConnectionTimeoutException(), "Connection timeout.");
            config.AddKey(section.Messages(), key.StompAckException(), "An ACK must have a subscription.");
            config.AddKey(section.Messages(), key.StompNackVersionException(), "A NACK is not supported on v1.0.");
            config.AddKey(section.Messages(), key.StompSubscribeException(), "A SUBSCRIBE must have an id defined.");
            config.AddKey(section.Messages(), key.StompNackSubscrptionExecption(), "A NACK must have a subscription.");
            config.AddKey(section.Messages(), key.StompUnsubscribeException(), "An UNSUBSCRIBE must have a destination or id, or both.");
   
        }

    }

}
