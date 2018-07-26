
namespace XAS.Core.Logging {

    /// <summary>
    /// Constants for XAS.Core.Logging.
    /// </summary>
    /// 

    /// <summary>
    /// The possible log types.
    /// </summary>
    /// 
    public enum LogType {

        /// <summary>
        /// Send log output to the console.
        /// </summary>
        /// 
        Console,

        /// <summary>
        /// Send log output to a file.
        /// </summary>
        /// 
        File,

        /// <summary>
        /// Send log output to a spool file as JSON.
        /// </summary>
        /// 
        Json,

        /// <summary>
        /// Send logout output to the Windows Event Log.
        /// </summary>
        /// 
        Event

    };

    /// <summary>
    /// Indicates the base logging level.
    /// </summary>
    /// 
    public enum LogLevel {

        /// <summary>
        /// Log level is info.
        /// </summary>
        /// 
        Info,

        /// <summary>
        /// Log level is warn.
        /// </summary>
        /// 
        Warn,

        /// <summary>
        /// Log level is error.
        /// </summary>
        /// 
        Error,

        /// <summary>
        /// Log level is fatal.
        /// </summary>
        /// 
        Fatal,

        /// <summary>
        /// Log level is debug.
        /// </summary>
        /// 
        Debug,

        /// <summary>
        /// Log level is trace.
        /// </summary>
        /// 
        Trace

    };

}

