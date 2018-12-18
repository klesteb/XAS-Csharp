using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using RestSharp;
using RestSharp.Authenticators;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using XAS.Rest.Client.Exceptions;
using XAS.Rest.Client.Serializers;

namespace XAS.Rest.Client {

    /// <summary>
    /// A REST based client.
    /// </summary>
    /// 
    public class Client {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly RestSharp.RestClient client = null;

        public String Url { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Client(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            client = new RestSharp.RestClient();

            client.AddHandler("text/html", new HtmlDeserializer());
            client.AddHandler("text/plain", new TextDeserializer());
            client.AddHandler("application/hal+json", new ApplicationHalDeserializer());
            client.AddHandler("application/problem+json", new ApplicationProblemDeserializer());

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(typeof(RestClient));

        }

        /// <summary>
        /// Make an async request call.
        /// </summary>
        /// <param name="request">A IRestRequest object.</param>
        /// <returns>The response from the call.</returns>
        /// 
        public async Task<IRestResponse<dynamic>> CallAsync(IRestRequest request) {

            return await Task.Run(() => Call(request));

        }

        /// <summary>
        /// Make an request call.
        /// </summary>
        /// <param name="request">A IRestRequest object.</param>
        /// <returns>The response from the call.</returns>
        /// 
        public IRestResponse<dynamic> Call(IRestRequest request) {

            dynamic response = null;

            client.BaseUrl = new Uri(this.Url);
            client.Authenticator = new HttpBasicAuthenticator(this.Username, this.Password);

            response = client.Execute<dynamic>(request);

            if ((response.StatusCode >= HttpStatusCode.OK) &&               // >= 200
                (response.StatusCode < HttpStatusCode.MultipleChoices)) {   // < 300

                return response;

            } else {

                if (response.ContentType == "application/problem+json") {

                    var error = response.Data;
                    var ex = new ApplicationProblemException(error.Detail);

                    ex.Data.Add("Title", error.Title);
                    ex.Data.Add("Status", error.Status);
                    ex.Data.Add("ErrorCode", error.ErrorCode);
                    ex.Data.Add("Detail", error.Detail);
                    ex.Data.Add("Type", error.Type);
                    ex.Data.Add("Exception", error.Exception);

                    throw ex;

                } else {

                    var ex = new ResponseException(response.StatusDescription);

                    ex.Data.Add("StatusCode", response.StatusCode);
                    ex.Data.Add("StatusDescription", response.StatusDescription);

                    throw ex;

                }

            }

        }

        /// <summary>
        /// Return the header value.
        /// </summary>
        /// <param name="response">An IRestReponse object.</param>
        /// <param name="header">The header name.</param>
        /// <returns>The value of the header.</returns>
        /// 
        public String GetHeader(IRestResponse response, String header) {

            string value = response.Headers.ToList()
                .Find(x => (x.Name == header))
                .Value.ToString();

            return value;

        }

    }

}
