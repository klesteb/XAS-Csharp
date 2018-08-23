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
    ///     ----------+-------------------------------------------------------------------------
    ///     Title       A short human readable summary of the problem type
    ///     Status      The HTTP status code
    ///     Detail      An human readable explaination specific to this occurence of the problem
    ///     Type        An absoulute URI that identifies the problem type.
    ///     Exception   A stack trace
    ///     ErrorCode   An application error code
    ///     
    /// </remarks>
    /// 
    public class ApplicationProblemException: Exception {

        public ApplicationProblemException(): base() { }
        public ApplicationProblemException(String message): base(message) { }
        public ApplicationProblemException(String message, Exception innerException): base(message, innerException) { }

    }

}
