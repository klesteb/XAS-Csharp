using System;

using XAS.Core.Configuration;

namespace DemoMicroServiceClient.Configuration.Extensions {

    /// <summary>
    /// Sections extensions.
    /// </summary>
    /// 
    public static class SectionExtensions {

        /// <summary>
        /// Get the key for the service section.
        /// </summary>
        /// 
        public static String Service(this Section junk) {
            return "service";
        }

    }

}
