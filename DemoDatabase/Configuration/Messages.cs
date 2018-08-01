
using XAS.Core.Configuration;
using XAS.Core.Configuration.Messages;

namespace DemoDatabase.Configuration {

    /// <summary>
    /// Load messages for DemoDatabase.
    /// </summary>
    /// 
    public class Messages: IMessages {

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
