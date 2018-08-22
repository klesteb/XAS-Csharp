using System;

using Nancy;
using Nancy.Validation;

using FluentValidation;

using XAS.Rest.Common.Errors;
using XAS.Rest.Server.Errors.Exceptions;

namespace XAS.Rest.Server.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications

    public static class ServiceErrorUtilities {

        public static ServiceErrorModel ExtractFromException(Exception exception, ServiceErrorModel defaultValue) {

            ServiceErrorModel result = defaultValue;

            if (exception != null) {

                if (exception is NotFoundErrorException) {

                    result.Detail = exception.Message.Trim();

                } else if (exception is ValidationException) {

                    result.Title = "Validation Exception";
                    result.Detail = "The following fields were invalid.";
                    result.Exception = exception.Message.Trim();

                } else {

                    result.Detail = exception.Message.Trim();
                    result.Exception = exception.StackTrace;

                }

            }

            return result;

        }

        public static ServiceErrorModel ValidationErrors(ModelValidationResult result) {

            var serviceError = new ServiceErrorModel {
                Title = "Validation Error",
                ErrorCode = ServiceErrorCode.UnprocessableEntity,
                Detail = "The following parameters have problems",
                Status = HttpStatusCode.UnprocessableEntity,
                Exception = result.Errors.ToString().Trim()
            };

            return serviceError;

        }

    }

}
