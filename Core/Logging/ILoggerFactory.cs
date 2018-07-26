using System;

namespace XAS.Core.Logging {
    
    /// <summary>
    /// An interface for the log factory.
    /// </summary>
    /// 
    public interface ILoggerFactory {

        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <param name="type">The LogType of the logger.</param>
        /// <returns>An initialized logger.</returns>
        /// 
        Logger Create(Type type);

    }

}
