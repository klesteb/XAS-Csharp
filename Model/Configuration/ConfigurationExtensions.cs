
using XAS.Core.Configuration;

namespace XAS.Model.Configuration {

    public static class ConfigurationExtensions {

        /// <summary>
        /// Populate the Messages section of the configuration, with Model specific messages.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public static void LoadModelMessages(this IConfiguration config) {

            var key = config.Key;
            var section = config.Section;

            config.AddKey(
                section.Messages(), 
                key.RepositoriesUpdateErrors(), 
                "Unable to update table, error: {0}, reason: {1}"
            );
            
            config.AddKey(
                section.Messages(), 
                key.RepositoriesValidationErrors(), 
                "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:"
            );

            config.AddKey(
                section.Messages(), 
                key.RepositoriesValidationProperties(), 
                "   Property: \"{0}\", Value: \"{1}\", Error: \"{2}\""
            );

        }

    }

}
