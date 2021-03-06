﻿using System;

using XAS.Core.Configuration;

namespace DemoMicroServiceServer.Configuration.Extensions {

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

        /// <summary>
        /// Get the key for the database section.
        /// </summary>
        /// 
        public static String Database(this Section junk) {
            return "database";
        }

    }

}
