using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using RestSharp;

using XAS.Core;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModelCommon.DataStructures;

namespace DemoMicroServiceClient {

    /// <summary>
    /// The client to interact with the REST service.
    /// </summary>
    /// 
    public class Client {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly XAS.Rest.Client.Client client = null;

        /// <summary>
        /// Get the port number of the Database service.
        /// </summary>
        /// 
        public Int32 Port { get; set; }

        /// <summary>
        /// Get the server's name for the Database service.
        /// </summary>
        ///
        public String Server { get; set; }

        /// <summary>
        /// Get the username to access the service.
        /// </summary>
        /// 
        public String Username { get; set; }

        /// <summary>
        /// Get the password to acces the service.
        /// </summary>
        /// 
        public String Password { get; set; }

        /// <summary>
        /// Get the url for the database service.
        /// </summary>
        /// 
        public String Url {
            get {
                return String.Format("http://{0}:{1}/", this.Server, this.Port);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler"> An IErrorHandler object.</param>
        /// <param name="logFactory">An ILogggerFactory object.</param>
        /// 
        public Client(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;

            this.log = logFactory.Create(typeof(Client));
            this.client = new XAS.Rest.Client.Client(config, handler, logFactory);

        }

        public DinosaurDTO Get(Int32 id) {

            log.Trace("Entering Get()");

            dynamic response = null;
            IRestRequest request = new RestRequest("/dinosaurs/{id}", Method.GET);

            client.Url = this.Url;
            client.Username = this.Username;
            client.Password = this.Password;

            request.RequestFormat = DataFormat.Json;
            request.AddParameter("id", id, ParameterType.UrlSegment);

            response = client.Call(request);
            var data = response.Data;

            log.Debug(String.Format("response: {0}", Utils.Dump(data)));

            log.Trace("Leaving Get()");

            return DemoMicroServiceClient.Create.DinosaurDTO(data);

        }

        public List<DinosaurDTO> List() {

            log.Trace("Entering Get()");

            dynamic response = null;
            var dtos = new List<DinosaurDTO>();
            IRestRequest request = new RestRequest("/dinosaurs/list", Method.GET);

            client.Url = this.Url;
            client.Username = this.Username;
            client.Password = this.Password;

            request.RequestFormat = DataFormat.Json;

            response = client.Call(request);

            log.Debug(String.Format("response: {0}", Utils.Dump(response.Data)));

            foreach (var datum in response.Data) {

                var data = DemoMicroServiceClient.Create.DinosaurDTO(datum);
                dtos.Add(data);

            }

            log.Trace("Leaving GetActions()");

            return dtos;

        }

        public Int32 Create(DinosaurPost data) {

            log.Trace("Entering Create()");

            Int32 jobId = 0;
            Match match = null;
            String location = "";
            IRestResponse response = null;
            Regex regex = new Regex(@".*(?:\D|^)(\d+)");
            IRestRequest request = new RestRequest("/dinosaurs", Method.POST);

            client.Url = this.Url;
            client.Username = this.Username;
            client.Password = this.Password;

            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);

            response = client.Call(request);
            location = client.GetHeader(response, "Location");

            // extract the job id from the location.

            if ((match = regex.Match(location)) != null) {

                log.Debug(String.Format("match value = {0}", match.Groups[1].Value));
                jobId = Convert.ToInt32(match.Groups[1].Value);

            }

            log.Trace("Leaving Create()");

            return jobId;

        }

        public Boolean Delete(Int32 id) {

            log.Trace("Entering Delete()");

            IRestResponse response = null;
            IRestRequest request = new RestRequest("/dinosaurs/{id}", Method.DELETE);

            client.Url = this.Url;
            client.Username = this.Username;
            client.Password = this.Password;

            request.RequestFormat = DataFormat.Json;
            request.AddParameter("id", id, ParameterType.UrlSegment);

            response = client.Call(request);

            log.Trace("Leaving Delete()");

            return true;

        }

        public Boolean Update(Int32 id, DinosaurUpdate data) {

            log.Trace("Entering Update()");

            Int32 jobId = 0;
            Match match = null;
            String location = "";
            IRestResponse response = null;
            Regex regex = new Regex(@".*(?:\D|^)(\d+)");
            IRestRequest request = new RestRequest("/dinosaurs/{id}", Method.PUT);

            client.Url = this.Url;
            client.Username = this.Username;
            client.Password = this.Password;

            request.RequestFormat = DataFormat.Json;
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddBody(data);

            response = client.Call(request);
            location = client.GetHeader(response, "Location");

            // extract the job id from the location.

            if ((match = regex.Match(location)) != null) {

                log.Debug(String.Format("match value = {0}", match.Groups[1].Value));
                jobId = Convert.ToInt32(match.Groups[1].Value);

            }

            log.Trace("Leaving Update()");

            return (jobId > 0);

        }

    }

}
