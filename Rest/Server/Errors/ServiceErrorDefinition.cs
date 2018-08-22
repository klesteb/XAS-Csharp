using Nancy;

using XAS.Rest.Common.Errors;

namespace XAS.Rest.Server.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications

    /// <summary>
    /// The defined error codes.
    /// </summary>
    /// <remarks>
    /// This maps HTTP protocol errors and internal errors to a common defined type. 
    /// </remarks>
    /// 
    public static class ServiceErrorDefinition {

        // application status codes

        public static ServiceErrorModel GeneralError = new ServiceErrorModel {
            Status = HttpStatusCode.BadRequest,
            ErrorCode = ServiceErrorCode.GeneralError,
            Title = "General Application Error",
            Detail = "An error occured during processing the request."
        };

        public static ServiceErrorModel InvalidTokenError = new ServiceErrorModel {
            Status = HttpStatusCode.BadRequest,
            ErrorCode = ServiceErrorCode.InvalidToken,
            Title = "Invalid API Error",
            Detail = "Invalid API Token."
        };

        // http status codes

        public static ServiceErrorModel BadRequest = new ServiceErrorModel {
            Status = HttpStatusCode.BadRequest,
            ErrorCode = ServiceErrorCode.BadRequest,
            Title = "400 Bad Request",
            Detail = "A bad request was made."
        };

        public static ServiceErrorModel Unauthorized = new ServiceErrorModel {
            Status = HttpStatusCode.Unauthorized,
            ErrorCode = ServiceErrorCode.Unauthorized,
            Title = "401 Unauthorized",
            Detail = "This request is not authorized."
        };

        public static ServiceErrorModel Forbidden = new ServiceErrorModel {
            Status = HttpStatusCode.Forbidden,
            ErrorCode = ServiceErrorCode.Forbidden,
            Title = "403 Permission",
            Detail = "Access to this resource is forbidden."
        };

        public static ServiceErrorModel NotFoundError = new ServiceErrorModel {
            Status = HttpStatusCode.NotFound,
            ErrorCode = ServiceErrorCode.NotFound,
            Title = "404 Not Found",
            Detail = "The requested entity was not found."
        };

        public static ServiceErrorModel NotAcceptable = new ServiceErrorModel {
            Status = HttpStatusCode.NotAcceptable,
            ErrorCode = ServiceErrorCode.NotAcceptable,
            Title = "406 Not Acceptable",
            Detail = "The requested content type is not acceptable."
        };

        public static ServiceErrorModel LengthRequired = new ServiceErrorModel {
            Status = HttpStatusCode.LengthRequired,
            ErrorCode = ServiceErrorCode.LengthRequired,
            Title = "411 Length Required",
            Detail = "The request must be chunked or have a content length."
        };

        public static ServiceErrorModel UnprocessableEntity  = new ServiceErrorModel {
            Status = HttpStatusCode.UnprocessableEntity,
            ErrorCode = ServiceErrorCode.UnprocessableEntity,
            Title = "422 Unprocessable Entity",
            Detail = "The request was unprocessable."
        };

        public static ServiceErrorModel InternalServerError = new ServiceErrorModel {
            Status = HttpStatusCode.InternalServerError,
            ErrorCode = ServiceErrorCode.InternalServerError,
            Title = "500 Internal Server Error",
            Detail = "There was an internal server error during processing the request."
        };

        public static ServiceErrorModel ServiceUnavailable = new ServiceErrorModel {
            Status = HttpStatusCode.ServiceUnavailable,
            ErrorCode = ServiceErrorCode.ServiceUnavailable,
            Title = "503 Service Unavailable Error",
            Detail = "The service is unavailable."
        };

    }

}
