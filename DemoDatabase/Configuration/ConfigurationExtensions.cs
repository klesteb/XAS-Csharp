
using XAS.Core.Configuration;

namespace DemoDatabase.Configuration {

    public static class ConfigurationExtensions {

        public static void LoadDemoDatabaseMessages(this IConfiguration config) {

            var key = config.Key;
            var section = config.Section;

            config.AddKey(section.Messages(), key.AddedTable(), "Added table: {0}");
            config.AddKey(section.Messages(), key.Created(), "Created database: {0}");
            config.AddKey(section.Messages(), key.UnknownModel(), "Unknown model: {0}");
            config.AddKey(section.Messages(), key.Implement(), "Implementing model: {0}");

        }

    }

}
