using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;

namespace XAS.Rest.Server.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications

    /// <summary>
    /// A class to handle the HTTP 400 status code.
    /// </summary>
    /// 
    public class StatusCodeHandler404: IStatusCodeHandler {

        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler404(IResponseNegotiator responseNegotiator) {

            this.responseNegotiator = responseNegotiator;

        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {

            return statusCode == HttpStatusCode.NotFound;

        }

        public void Handle(HttpStatusCode statusCode, NancyContext context) {

            context.NegotiationContext = new NegotiationContext();

            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(ServiceErrorDefinition.NotFoundError.Status)
                .WithContentType("application/problem+json")
                .WithModel(ServiceErrorDefinition.NotFoundError)
            ;

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);

        }

    }

}
