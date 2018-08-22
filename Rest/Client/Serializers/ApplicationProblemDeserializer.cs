
using RestSharp;
using RestSharp.Deserializers;

using Newtonsoft.Json;
using XAS.Rest.Common.Errors;

namespace XAS.Rest.Client.Serializers {

    public class ApplicationProblemDeserializer: IDeserializer {

        public string RootElement { get; set; }
        public string DateFormat { get; set; }
        public string Namespace { get; set; }

        public T Deserialize<T>(IRestResponse response) {

            return (dynamic)JsonConvert.DeserializeObject<ServiceErrorModel>(response.Content);

        }

    }
}

