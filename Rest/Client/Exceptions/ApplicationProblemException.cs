using System;

namespace XAS.Rest.Client.Exceptions {

    public class ApplicationProblemException: Exception {

        public ApplicationProblemException(): base() { }
        public ApplicationProblemException(String message): base(message) { }
        public ApplicationProblemException(String message, Exception innerException): base(message, innerException) { }

    }

}
