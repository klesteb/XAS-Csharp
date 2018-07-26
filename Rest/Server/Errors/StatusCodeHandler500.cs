using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;

namespace XAS.Rest.Server.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications

    /// <summary>
    /// A class to handle the HTTP 501 status code.
    /// </summary>
    /// 
    public class StatusCodeHandler500: IStatusCodeHandler {

        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler500(IResponseNegotiator responseNegotiator) {

            this.responseNegotiator = responseNegotiator;

        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {

            return statusCode == HttpStatusCode.InternalServerError;

        }

        public void Handle(HttpStatusCode statusCode, NancyContext context) {

            context.NegotiationContext = new NegotiationContext();

            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(ServiceErrorDefinition.InternalServerError.Status)
                .WithContentType("application/problem+json")
                .WithModel(ServiceErrorDefinition.InternalServerError)
            ;

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);

        }

    }

}
