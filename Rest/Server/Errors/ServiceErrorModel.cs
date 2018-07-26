using Nancy;

namespace XAS.Rest.Server.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications
    //
    // patterned after: https://tools.ietf.org/html/rfc7807
    //

    /// <summary>
    /// A class to define error responses.
    /// </summary>
    /// 
    public class ServiceErrorModel {

        /// <summary>
        /// Get/Set the error code.
        /// </summary>
        /// 
        public ServiceErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Get/Set the HTTP status code.
        /// </summary>
        /// 
        public HttpStatusCode Status { get; set; }

        /// <summary>
        /// Get/Set details about the error.
        /// </summary>
        /// 
        public string Detail { get; set; }

        /// <summary>
        /// Get/Set the possible exception stack trace.
        /// </summary>
        /// 
        public string Exception { get; set; }

        /// <summary>
        /// Get/Set a title.
        /// </summary>
        /// 
        public string Title { get; set; }

        /// <summary>
        /// Get/Set the type of error (a possible URL link).
        /// </summary>
        /// 
        public string Type { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public ServiceErrorModel() {

            this.Type = "about:blank";

        }

    }

}
