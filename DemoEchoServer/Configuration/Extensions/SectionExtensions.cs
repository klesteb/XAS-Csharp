using System;

using XAS.Core.Configuration;

namespace DemoEchoServer.Configuration.Extensions {

    /// <summary>
    /// Section extension for DemoEchoServer.
    /// </summary>
    /// 
    public static class SectionExtensions {

        /// <summary>
        /// Get the key for the SSL section.
        /// </summary>
        /// 
        public static String SSL(this Section junk) {
            return "ssl";
        }

        /// <summary>
        /// Get the key for the server section.
        /// </summary>
        /// 
        public static String Server(this Section junk) {
            return "server";
        }

    }

}
