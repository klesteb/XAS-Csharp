using System;
using System.Runtime.Serialization;

namespace XAS.App.Exceptions {

    /// <summary>
    /// An exception for missing ConfigFiles in config files.
    /// </summary>
    /// 
    [Serializable]
    public class ConfigFileMissingException: Exception {

        /// <summary>
        /// Construtor.
        /// </summary>
        /// 
        public ConfigFileMissingException() : base() { }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="message">A message associated with the exception.</param>
        /// 
        public ConfigFileMissingException(string message) : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">A format for the message.</param>
        /// <param name="args">Arguments for the format.</param>
        /// 
        public ConfigFileMissingException(string format, params object[] args) :
            base(String.Format(format, args)) {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">A message associated with the execption.</param>
        /// <param name="innerException">The inner execption to be used.</param>
        /// 
        public ConfigFileMissingException(string message, Exception innerException) :
            base(message, innerException) {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">A foramt for the message.</param>
        /// <param name="innerException">The inner execption to be used.</param>
        /// <param name="args">Arguments for the format.</param>
        /// 
        public ConfigFileMissingException(string format, Exception innerException, params object[] args) :
            base(String.Format(format, args), innerException) {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        public ConfigFileMissingException(SerializationInfo info, StreamingContext context) :
            base(info, context) {
        }

    }

}
