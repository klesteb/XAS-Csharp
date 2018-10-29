using System;

using XAS.Core.Configuration;

namespace ServiceSupervisor.Configuration.Extensions {

    /// <summary>
    /// Section extension for DemoMicroServiceServer.
    /// </summary>
    /// 
    public static class SectionExtensions {

        /// <summary>
        /// Get the key for the web section.
        /// </summary>
        /// 
        public static String Web(this Section junk) {
            return "web";
        }

    }

}
