
using RestSharp;
using RestSharp.Deserializers;

namespace XAS.Rest.Client.Serializers {

    /// <summary>
    /// Deserialze a text/plain document.
    /// </summary>
    /// 
    public class TextPlainDeserializer: IDeserializer {

        public string RootElement { get; set; }
        public string DateFormat { get; set; }
        public string Namespace { get; set; }

        public T Deserialize<T>(IRestResponse response) {

            return (dynamic)response.Content;

        }

    }

}
