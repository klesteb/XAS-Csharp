using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;

namespace XAS.Rest.Server.Errors {

    /// <summary>
    /// A class to handle the HTTP 411 status code.
    /// </summary>
    /// 
    public class StatusCodeHandler411: IStatusCodeHandler {

        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler411(IResponseNegotiator responseNegotiator) {

            this.responseNegotiator = responseNegotiator;

        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {

            return statusCode == HttpStatusCode.LengthRequired;

        }

        public void Handle(HttpStatusCode statusCode, NancyContext context) {

            context.NegotiationContext = new NegotiationContext();

            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(ServiceErrorDefinition.LengthRequired.Status)
                .WithContentType("application/problem+json")
                .WithModel(ServiceErrorDefinition.LengthRequired)
            ;

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);

        }

    }

}
