using System;

using FluentValidation;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;

using XAS.Rest.Server.Errors.Exceptions;

namespace XAS.Rest.Server.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications

    /// <summary>
    /// A class to implement custom error handling for Nancy.
    /// </summary>
    /// 
    public static class CustomErrorHandler {

        /// <summary>
        /// Add a custom error handler to the OnError pipeline.
        /// </summary>
        /// <param name="pipelines">A Nancy pipeline.</param>
        /// <param name="responseNegotiator">A Nancy response negotiator.</param>
        /// 
        public static void Enable(IPipelines pipelines, IResponseNegotiator responseNegotiator) {

            if (pipelines == null) {

                throw new ArgumentNullException("pipelines");

            }


            if (responseNegotiator == null) {

                throw new ArgumentNullException("responseNegotiator");

            }

            pipelines.OnError += (context, exception) => HandleException(context, exception, responseNegotiator);

        }

        private static Response HandleException(NancyContext context, Exception exception, IResponseNegotiator responseNegotiator) {

            return CreateNegotiatedResponse(context, responseNegotiator, exception);

        }

        private static Response CreateNegotiatedResponse(NancyContext context, IResponseNegotiator responseNegotiator, Exception exception) {

            ServiceErrorModel serviceError;

            if (exception is NotFoundErrorException) {

                serviceError = ServiceErrorUtilities.ExtractFromException(
                    exception, 
                    ServiceErrorDefinition.NotFoundError
                );

            } else if (exception is NotProcessableErrorException) {

                serviceError = ServiceErrorUtilities.ExtractFromException(
                    exception, 
                    ServiceErrorDefinition.UnprocessableEntity
                );

            } else if (exception is ValidationException) {

                serviceError = ServiceErrorUtilities.ExtractFromException(
                    exception,
                    ServiceErrorDefinition.UnprocessableEntity
                );
                                
            } else {
            
                serviceError = ServiceErrorUtilities.ExtractFromException(
                    exception, 
                    ServiceErrorDefinition.InternalServerError
                );

            }

            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(serviceError.Status)
                .WithContentType("application/problem+json")
                .WithModel(serviceError)
            ;

            return responseNegotiator.NegotiateResponse(negotiator, context);

        }

    }

}
