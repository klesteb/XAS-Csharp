using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;

namespace XAS.Rest.Server.Errors {

    /// <summary>
    /// A class to handle the HTTP 406 status code.
    /// </summary>
    /// 
    public class StatusCodeHandler406: IStatusCodeHandler {

        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler406(IResponseNegotiator responseNegotiator) {

            this.responseNegotiator = responseNegotiator;

        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {

            return statusCode == HttpStatusCode.NotAcceptable;

        }

        public void Handle(HttpStatusCode statusCode, NancyContext context) {

            context.NegotiationContext = new NegotiationContext();

            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(ServiceErrorDefinition.NotAcceptable.Status)
                .WithContentType("application/problem+json")
                .WithModel(ServiceErrorDefinition.NotAcceptable)
            ;

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);

        }

    }

}
