
using XAS.Core.Configuration;
using XAS.Core.Configuration.Messages;
using XAS.Core.Configuration.Extensions;
using XAS.Network.Configuration.Extensions;

namespace XAS.Network.Configuration.Messages {

    /// <summary>
    /// Load the messages for Network.
    /// </summary>
    /// 
    public class Messages: IMessages {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Messages() { }

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

            config.AddKey(section.Messages(), key.ClientProblems(), "Host: {0}, Port: {1} has problems");
            config.AddKey(section.Messages(), key.ClientConnect(), "Client {0} on port {1} has connected.");
            config.AddKey(section.Messages(), key.ClientInactive(), "Client {0} on port {1} has been disconected for inactivity.");
            config.AddKey(section.Messages(), key.ClientDeadSocket(), "Clinet {0} on port {1} had been disconnected for a dead socker.");
            config.AddKey(section.Messages(), key.ClientSSLValidation(), "Client {0} on port {1} had been disconnected for bad ssl validation.");

            config.AddKey(section.Messages(), key.ServerDisconnect(), "Disconneted from {0}, trying to reconnect.");
            config.AddKey(section.Messages(), key.ServerConnect(), "Connected to {0} on port {0}.");

        }

    }

}
