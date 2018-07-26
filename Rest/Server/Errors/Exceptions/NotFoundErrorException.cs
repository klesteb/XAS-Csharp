using System;

namespace XAS.Rest.Server.Errors.Exceptions {

    public class NotFoundErrorException: Exception, IHasServiceError {

        public NotFoundErrorException(): base() { }

        public NotFoundErrorException(string message): base(message) { }

        public NotFoundErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        public ServiceErrorModel HttpServiceError {
            get { return ServiceErrorDefinition.NotFoundError; }
        }

    }

}
