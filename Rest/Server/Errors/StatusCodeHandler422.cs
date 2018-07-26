using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;

namespace XAS.Rest.Server.Errors {

     /// <summary>
     /// A class to handle the HTTP 400 status code.
     /// </summary>
     /// 
    public class StatusCodeHandler422: IStatusCodeHandler {

        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler422(IResponseNegotiator responseNegotiator) {

            this.responseNegotiator = responseNegotiator;

        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {

            return statusCode == HttpStatusCode.UnprocessableEntity;

        }

        public void Handle(HttpStatusCode statusCode, NancyContext context) {

            context.NegotiationContext = new NegotiationContext();

            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(ServiceErrorDefinition.UnprocessableEntity.Status)
                .WithContentType("application/problem+json")
                .WithModel(ServiceErrorDefinition.UnprocessableEntity)
            ;

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);

        }

    }

}
