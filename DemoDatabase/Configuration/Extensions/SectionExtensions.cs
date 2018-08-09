using System;

using XAS.Core.Configuration;

namespace DemoDatabase.Configuration.Extensions {

    /// <summary>
    /// Sections extensions.
    /// </summary>
    /// 
    public static class SectionExtensions {

        /// <summary>
        /// Get the key for the database section.
        /// </summary>
        /// 
        public static String Database(this Section junk) {
            return "database";
        }

    }

}
