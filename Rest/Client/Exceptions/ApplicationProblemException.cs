using System;

namespace XAS.Rest.Client.Exceptions {

    /// <summary>
    /// An exception to handle "application/problem+json".
    /// </summary>
    /// <remarks>
    /// 
    /// The Data property is used to capture the complete error messager from the server.
    /// The following properties are used.
    /// 
    ///     Key:        Value:
    ///     ----------+-----------------------------
    ///     Title       The title of the error
    ///     Status      The error status
    ///     ErrorCode   The error code
    ///     Detail      Error details
    ///     Type        A url to possible useful help 
    ///     Exception   A stack trace
    ///     
    /// </remarks>
    /// 
    public class ApplicationProblemException: Exception {

        public ApplicationProblemException(): base() { }
        public ApplicationProblemException(String message): base(message) { }
        public ApplicationProblemException(String message, Exception innerException): base(message, innerException) { }

    }

}
