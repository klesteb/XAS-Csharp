
using XAS.Core.Configuration;
using XAS.Core.Configuration.Messages;
using XAS.Core.Configuration.Extensions;

using ServiceSpooler.Configuration.Extensions;

namespace ServiceSpooler.Configuration.Messages {

    /// <summary>
    /// Load messages for ServiceSpooler.
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

            config.AddKey(section.Messages(), key.UnlinkFile(), "Removed file \"{0}\".");
            config.AddKey(section.Messages(), key.Disconnected(), "Disconnected from {0}.");
            config.AddKey(section.Messages(), key.CorruptFile(), "Corrupted file \"{0}\".");
            config.AddKey(section.Messages(), key.Connected(), "Connected to {0} on port {1}.");
            config.AddKey(section.Messages(), key.ProtocolError(), "Protocol error: {0} - {1}.");
            config.AddKey(section.Messages(), key.NoData(), "Empty file \"{0}\", for queue {1}.");
            config.AddKey(section.Messages(), key.WatchDirectory(), "Watching directory \"{0}\".");
            config.AddKey(section.Messages(), key.NoDirectory(), "Directory \"{0}\" was not found.");
            config.AddKey(section.Messages(), key.FileFound(), "Found file \"{0}\", queuing to {1}.");
            config.AddKey(section.Messages(), key.UnknownFile(), "\"{0}\" is from an unwatched directory.");
            config.AddKey(section.Messages(), key.MonitorDirectory(), "Monitoring directory \"{0}\" for orphans.");

        }

    }

}
