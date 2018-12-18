
using HtmlAgilityPack;

using RestSharp;
using RestSharp.Deserializers;

namespace XAS.Rest.Client.Serializers {

    /// <summary>
    /// Deserialze a text/html document into a DOM representation.
    /// </summary>
    /// 
    public class HtmlDeserializer: IDeserializer {

        public string RootElement { get; set; }
        public string DateFormat { get; set; }
        public string Namespace { get; set; }
        
        public T Deserialize<T>(IRestResponse response) {

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response.Content);

            return (dynamic)htmlDoc;

        }

    }

}
