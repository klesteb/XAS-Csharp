using System;
using System.Runtime.Serialization;

namespace XAS.Core.Exceptions {

    /// <summary>
    /// An exception for invalid options.
    /// </summary>
    /// 
    [Serializable]
    public class InvalidConfigException: Exception {

        /// <summary>
        /// Construtor.
        /// </summary>
        /// 
        public InvalidConfigException() : base() { }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="message">A message associated with the exception.</param>
        /// 
        public InvalidConfigException(string message) : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">A format for the message.</param>
        /// <param name="args">Arguments for the format.</param>
        /// 
        public InvalidConfigException(string format, params object[] args) :
            base(String.Format(format, args)) {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">A message associated with the execption.</param>
        /// <param name="innerException">The inner execption to be used.</param>
        /// 
        public InvalidConfigException(string message, Exception innerException) :
            base(message, innerException) {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">A foramt for the message.</param>
        /// <param name="innerException">The inner execption to be used.</param>
        /// <param name="args">Arguments for the format.</param>
        /// 
        public InvalidConfigException(string format, Exception innerException, params object[] args) :
            base(String.Format(format, args), innerException) {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        public InvalidConfigException(SerializationInfo info, StreamingContext context) :
            base(info, context) {

        }

    }

}
