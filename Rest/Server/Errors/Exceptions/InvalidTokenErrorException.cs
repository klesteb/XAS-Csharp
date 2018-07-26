using System;

namespace XAS.Rest.Server.Errors.Exceptions {

    public class InvalidTokenErrorException:Exception, IHasServiceError {

        public InvalidTokenErrorException(): base() { }

        public InvalidTokenErrorException(string message): base(message) { }

        public InvalidTokenErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        public ServiceErrorModel HttpServiceError {
            get { return ServiceErrorDefinition.InvalidTokenError; }
        }

    }

}
