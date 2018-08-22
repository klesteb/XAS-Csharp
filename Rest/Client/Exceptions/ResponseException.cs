using System;

namespace XAS.Rest.Client.Exceptions {

    /// <summary>
    /// An exception to HTTP response exceptions.
    /// </summary>
    /// <remarks>
    /// 
    /// The Data property is used to capture the complete error message from the server.
    /// The following properties are used.
    /// 
    ///     Key:                Value:
    ///     ------------------+-----------------------------
    ///     StatusCode          The HTTP status code
    ///     StatusDescription   The description of the error
    ///     
    /// </remarks>
    /// 
    public class ResponseException: Exception {

        public ResponseException(): base() { }
        public ResponseException(String message): base(message) { }
        public ResponseException(String message, Exception innerException): base(message, innerException) { }

    }

}
