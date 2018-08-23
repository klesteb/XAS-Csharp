using System;

using XAS.Core.Configuration;

namespace ServiceSpooler.Configuration.Extensions {

    /// <summary>
    /// Section extension for ServiceSpooler.
    /// </summary>
    /// 
    public static class SectionExtensions {

        public static String MessageQueue(this Section junk) {
            return "message-queue";
        }

    }

}
