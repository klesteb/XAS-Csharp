using System;

namespace XAS.Rest.Server.Errors.Exceptions {

    public class InternalServerErrorException: Exception, IHasServiceError {

        public InternalServerErrorException(): base() { }

        public InternalServerErrorException(string message): base(message) { }

        public InternalServerErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        public ServiceErrorModel HttpServiceError {
            get { return ServiceErrorDefinition.InternalServerError; }
        }

    }

}
