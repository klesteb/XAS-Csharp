
using XAS.Core.Configuration;
using XAS.Core.Configuration.Messages;
using XAS.Core.Configuration.Extensions;

using DemoDatabase.Configuration.Extensions;

namespace DemoDatabase.Configuration.Messages {

    /// <summary>
    /// Load messages for DemoDatabase.
    /// </summary>
    /// 
    public class Messages: IMessages {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Messages() { }

        /// <summary>
        /// Load Messages.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public void Load(IConfiguration config) {

            var key = config.Key;
            var section = config.Section;

            config.AddKey(section.Messages(), key.AddedTable(), "Added table: {0}");
            config.AddKey(section.Messages(), key.Created(), "Created database: {0}");
            config.AddKey(section.Messages(), key.UnknownModel(), "Unknown model: {0}");
            config.AddKey(section.Messages(), key.Implement(), "Implementing model: {0}");

        }

    }

}
