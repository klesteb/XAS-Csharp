using System;

namespace XAS.Rest.Server.Errors.Exceptions {

    public class NotProcessableErrorException: Exception, IHasServiceError {

        public NotProcessableErrorException(): base() { }

        public NotProcessableErrorException(string message): base(message) { }

        public NotProcessableErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        public ServiceErrorModel HttpServiceError {
            get { return ServiceErrorDefinition.UnprocessableEntity; }
        }

    }

}
