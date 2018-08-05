using System.IO;

using XAS.Core.Configuration;
using XAS.Core.Configuration.Messages;
using XAS.Core.Configuration.Extensions;

using DemoMicroServiceServer.Configuration.Extensions;

namespace DemoMicroServiceServer.Configuration.Messages {

    /// <summary>
    /// Load messages for the DemoMicroServiceSErver.
    /// </summary>
    /// 
    public class Messages: IMessages {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Messages() { }

        /// <summary>
        /// Load messages.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public void Load(IConfiguration config) {

            var key = config.Key;
            var section = config.Section;
            string libDir = config.GetValue(section.Environment(), key.LibDir());

            config.CreateSection(section.Web());
            config.AddKey(section.Web(), key.Address(), "http://localhost:8080");
            config.AddKey(section.Web(), key.WebRootPath(), Path.Combine(libDir, "web"));
            config.AddKey(section.Web(), key.EnableClientCertificates(), false.ToString());

        }

    }

}
