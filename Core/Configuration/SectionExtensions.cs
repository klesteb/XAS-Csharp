using System;

namespace XAS.Core.Configuration {

    public static class SectionExtensions {

        /// <summary>
        /// Get the key for the environment section.
        /// </summary>
        /// 
        public static String Environment(this Section junk) {
            return "environment";
        }

        /// <summary>
        /// Get the key for the messages section.
        /// </summary>
        /// 
        public static String Messages(this Section junk) {
            return "messages";
        }

        /// <summary>
        /// Get the key for the applicaton section.
        /// </summary>
        /// 
        public static String Application(this Section junk) {
            return "application";
        }

    }

}
