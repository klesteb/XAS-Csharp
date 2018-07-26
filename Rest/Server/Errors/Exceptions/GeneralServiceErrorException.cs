using System;

namespace XAS.Rest.Server.Errors.Exceptions {

    public class GeneralServiceErrorException: Exception, IHasServiceError {

        public GeneralServiceErrorException(): base() { }

        public GeneralServiceErrorException(string message): base(message) { }

        public GeneralServiceErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        public ServiceErrorModel HttpServiceError {
            get { return ServiceErrorDefinition.GeneralError; }
        }

    }

}
