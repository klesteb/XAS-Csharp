using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAS.Core.Configuration.Messages {
    
    /// <summary>
    /// Load default core messages.
    /// </summary>
    /// 
    public class Messages: IMessages {

        /// <summary>
        /// Load the messages.
        /// </summary>
        /// <param name="config">An IConfguration object.</param>
        /// 
        public void Load(IConfiguration config) {

            var key = config.Key;
            var section = config.Section;

            config.AddKey(section.Messages(), key.StartRun(), "Start run");
            config.AddKey(section.Messages(), key.StopRun(), "Stop run");
            config.AddKey(section.Messages(), key.StartUp(), "Starting up");
            config.AddKey(section.Messages(), key.ShutDown(), "Shutting down");
            config.AddKey(section.Messages(), key.FileMissing(), "{0} is missing");
            config.AddKey(section.Messages(), key.ArgumentIsNull(), "The requested argument is null: \"{0}\"");
            config.AddKey(section.Messages(), key.InvalidMimeType(), "Requested mime type is not valid: \"{0}\"");
            config.AddKey(section.Messages(), key.MimeTypeNotRegistered(), "Requested mime type is not registered: \"{0}\"");

        }

    }

}
