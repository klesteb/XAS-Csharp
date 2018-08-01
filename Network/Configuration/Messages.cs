
using XAS.Core.Configuration;
using XAS.Core.Configuration.Messages;

namespace XAS.Network.Configuration {

    /// <summary>
    /// Load the messages for Network.
    /// </summary>
    /// 
    public class Messages: IMessages {

        /// <summary>
        /// Load messages.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public void Load(IConfiguration config) {

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
