using System;

namespace XAS.Rest.Client.Exceptions {

    public class ResponseException: Exception {

        public ResponseException(): base() { }
        public ResponseException(String message): base(message) { }
        public ResponseException(String message, Exception innerException): base(message, innerException) { }

    }

}
