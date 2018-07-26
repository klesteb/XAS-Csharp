using System;
using System.Collections.Generic;

namespace XAS.Rest.Client.Serializers.Lib {

    public class HalResponse: JsonResponse {

        public Boolean HasLinks {
            get {
                return properties.ContainsKey("Links");
            }
        }

        public Boolean HasEmbedded {
            get {
                return properties.ContainsKey("Embedded");
            }
        }

        public new List<String> Properties {
            get {
                List<string> keys = new List<string>();
                foreach (var key in properties.Keys) {
                    if ((key != "Links") && (key != "Embedded")) {
                        keys.Add(key);
                    }
                }
                return keys;
            }
        }

    }

}
