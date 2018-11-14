
using XAS.Core.Configuration;
using XAS.Core.Configuration.Messages;
using XAS.Core.Configuration.Extensions;

using ServiceSupervisor.Configuration.Extensions;

namespace ServiceSupervisor.Configuration.Messages {

    /// <summary>
    /// Load messages for ServiceSupervisor.
    /// </summary>
    /// 
    public class Messages: IMessages {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Messages() { }

        /// <summary>
        /// Load Messages.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// 
        public void Load(IConfiguration config) {

            var key = config.Key;
            var section = config.Section;

            config.AddKey(section.Messages(), key.GET3(), "Processing GET(/{0}/{1}) for {2}");
            config.AddKey(section.Messages(), key.PUT4(), "Processing PUT(/{0}/{1}/{2}) for {3}");
            config.AddKey(section.Messages(), key.OPTIONS3(), "Processing OPTIONS(/{0}/{1}) for {2}");
            config.AddKey(section.Messages(), key.OPTIONS4(), "Processing OPTIONS(/{0}/{1}/{2}) for {3}");
            config.AddKey(section.Messages(), key.PUT_NoStart(), "Result of PUT(/{0}/{1}/start) for {2}, unable to queue start");
            config.AddKey(section.Messages(), key.PUT_NoStop(), "Result of PUT(/{0}/{1}/stop) for {2}, unable to queue stop");

        }

    }

}
