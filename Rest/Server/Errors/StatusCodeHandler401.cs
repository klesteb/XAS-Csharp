using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;

namespace XAS.Rest.Server.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications

    /// <summary>
    /// A class to handle the HTTP 401 status code.
    /// </summary>
    /// 
    public class StatusCodeHandler401: IStatusCodeHandler {

        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler401(IResponseNegotiator responseNegotiator) {

            this.responseNegotiator = responseNegotiator;

        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {

            return statusCode == HttpStatusCode.Unauthorized;

        }

        public void Handle(HttpStatusCode statusCode, NancyContext context) {

            context.NegotiationContext = new NegotiationContext();

            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(ServiceErrorDefinition.Unauthorized.Status)
                .WithContentType("application/problem+json")
                .WithModel(ServiceErrorDefinition.Unauthorized)
            ;

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);

        }

    }

}
