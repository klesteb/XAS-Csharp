using System;

using XAS.Core.Configuration.Extensions;

namespace XAS.Core.Configuration.Messages {
    
    /// <summary>
    /// Load default core messages.
    /// </summary>
    /// 
    public class Messages: IMessages {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Messages() { }

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

            config.AddKey(section.Messages(), key.RoboCopyLevel1(), "No errors occurred, and no copying was done");
            config.AddKey(section.Messages(), key.RoboCopyLevel2(), "One or more files were copied successfully");
            config.AddKey(section.Messages(), key.RoboCopyLevel3(), "Some extra files or directories were detected");
            config.AddKey(section.Messages(), key.RoboCopyLevel4(), "Some mismatched files or directories were detected");
            config.AddKey(section.Messages(), key.RoboCopyLevel5(), "Some files or directories could not be copied");
            config.AddKey(section.Messages(), key.RoboCopyLevel6(), "Serious error robocopy did not copy any files");

        }

    }

}
