using System;

using RestSharp;
using RestSharp.Deserializers;

using XAS.Rest.Client.Serializers.Lib;

namespace XAS.Rest.Client.Serializers {

    public class ApplicationHalDeserializer: IDeserializer {

        private HalParser parser = null;

        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public string RootElement { get; set; }

        public ApplicationHalDeserializer() {

            parser = new HalParser();

        }
        
        public T Deserialize<T>(IRestResponse response) {
        
            return (dynamic)parser.Parse(response.Content);

        }

    }

}
