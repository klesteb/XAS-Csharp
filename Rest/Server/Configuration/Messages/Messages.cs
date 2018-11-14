
using XAS.Core.Configuration;
using XAS.Core.Configuration.Messages;
using XAS.Core.Configuration.Extensions;

using XAS.Rest.Server.Configuration.Extensions;

namespace XAS.Rest.Server.Configuration.Messages {

    /// <summary>
    /// Load messages for DemoDatabase.
    /// </summary>
    /// 
    public class Messages:IMessages {

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

            config.AddKey(section.Messages(), key.DELETE(), "Processing DELETE(/{0}/{1}) for {2}");
            config.AddKey(section.Messages(), key.DELETE_NoDelete(), "Result of DELETE(/{0}/{1}) for {2}, unable to delete");
            config.AddKey(section.Messages(), key.GET(), "Processing GET(/{0}) for {1}");
            config.AddKey(section.Messages(), key.GETS(), "Processing GET(/{0}/{1}) for {2}");
            config.AddKey(section.Messages(), key.OPTION(), "Processing OPTIONS(/{0}/{1}) for {2}");
            config.AddKey(section.Messages(), key.OPTIONA(), "Processing OPTIONS(/{0}/{1}/{2}/{3}) for {4}");
            config.AddKey(section.Messages(), key.OPTIONS(), "Processing OPTIONS(/{0}) for {1}");
            config.AddKey(section.Messages(), key.POST(), "Processing POST(/{0}) for {1}");
            config.AddKey(section.Messages(), key.POST_NoCreate(), "Result of POST(/{0}) for {1}, unable to create");
            config.AddKey(section.Messages(), key.PUT(), "Processing PUT(/{0}/{1}) for {2})");
            config.AddKey(section.Messages(), key.PUTA(), "Processing PUT(/{0}/{1}/{2}/{3}) for {4}");
            config.AddKey(section.Messages(), key.PUT_NoDelete(), "Result of PUT(/{0}/{1}) for {2}, unable to delete");
            config.AddKey(section.Messages(), key.PUT_NoValidate(), "Result of PUT(/{0}/{1}) for {2}, unable to validate");
            config.AddKey(section.Messages(), key.PUT_NoUpdate(), "Result of PUT(/{0}/{1}) for {2}, unable to update");

        }

    }

}
