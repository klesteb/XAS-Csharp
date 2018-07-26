using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;

namespace XAS.Rest.Server.Errors {

    /// <summary>
    /// A class to handle the HTTP 503 status code.
    /// </summary>
    /// 
    public class StatusCodeHandler503: IStatusCodeHandler {

        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler503(IResponseNegotiator responseNegotiator) {

            this.responseNegotiator = responseNegotiator;

        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {

            return statusCode == HttpStatusCode.InternalServerError;

        }

        public void Handle(HttpStatusCode statusCode, NancyContext context) {

            context.NegotiationContext = new NegotiationContext();

            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(ServiceErrorDefinition.ServiceUnavailable.Status)
                .WithContentType("application/problem+json")
                .WithModel(ServiceErrorDefinition.ServiceUnavailable)
            ;

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);

        }

    }

}
