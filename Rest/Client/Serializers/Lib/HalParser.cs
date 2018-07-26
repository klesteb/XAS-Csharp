using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XAS.Rest.Client.Serializers.Lib {

    public class HalParser {

        public HalParser() { }
        
        public dynamic Parse(String json) {

            if (String.IsNullOrEmpty(json)) {

                throw new ArgumentNullException(nameof(json));

            }

            var obj = JObject.Parse(json);
            dynamic resource = ParseResource(obj);

            return resource;

        }

        private static HalResponse ParseResource(JObject outer) {

            dynamic response = new HalResponse();

            ParseResourceObject(outer, response);

            return response;

        }

        private static void ParseResourceObject(JObject outer, dynamic response) {

            foreach (var inner in outer.Properties()) {

                var type = inner.Value.Type.ToString();

                if (inner.Value.Type == JTokenType.Object) {

                    var value = (JObject)inner.Value;

                    switch (inner.Name) {
                        case "_links":
                            List<LinkObject> links = new List<LinkObject>();
                            links.AddRange(ParseObjectOrArrayOfObjects(value, ParseLinkObject));
                            response.Links = links;
                            break;

                        case "_embedded":
                            List<Object> embedded = new List<Object>();
                            embedded.AddRange(ParseObjectOrArrayOfObjects(value, ParseEmbeddedObject));
                            response.Embedded = embedded;
                            break;

                        default:
                            response.AddProperty(inner.Name, new StateValue(value.ToString(Formatting.Indented), type));
                            break;
                    }

                } else if (inner.Value.Type == JTokenType.Array) {

                    var value = (JObject)inner.Value;

                    switch (inner.Name) {
                        case "_links":
                        case "_embedded":
                            if (inner.Value.Type != JTokenType.Null)
                                throw new FormatException(string.Format("Invalid value for {0}: {1}", inner.Name, value));
                            break;

                        default:
                            var array = new List<object>();
                            array.AddRange(ParseObjectOrArrayOfObjects(value, ParseArrayObject));
                            response.AddProperty(inner.Name, new StateValue(array, type));
                            break;
                    }

                } else {

                    var value = inner.Value.ToString();

                    switch (inner.Name) {
                        case "_links":
                        case "_embedded":
                            if (inner.Value.Type != JTokenType.Null)
                                throw new FormatException(string.Format("Invalid value for {0}: {1}", inner.Name, value));
                            break;

                        default:
                            response.AddProperty(inner.Name, new StateValue(value, type));
                            break;
                    }

                }

            }

        }

        private static HalResponse ParseEmbeddedObject(JObject outer, string rel) {

            dynamic response = new HalResponse();

            ParseResourceObject(outer, response);

            return response;

        }

        private static HalResponse ParseArrayObject(JObject outer, string rel) {

            dynamic response = new HalResponse();

            ParseResourceObject(outer, response);

            return response;

        }

        private static LinkObject ParseLinkObject(JObject outer, string rel) {

            var link = new LinkObject { Rel = rel };
            string href = null;

            foreach (var inner in outer.Properties()) {

                var value = inner.Value.ToString();

                if (string.IsNullOrEmpty(value)) {

                    continue; // nothing to assign, just leave the default value ...

                }

                var attribute = inner.Name.ToLowerInvariant();

                switch (attribute) {
                    case "href":
                        href = value;
                        break;

                    case "templated":
                        link.Templated = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;

                    case "type":
                        link.Type = value;
                        break;

                    case "deprecation":
                        link.SetDeprecation(value);
                        break;

                    case "name":
                        link.Name = value;
                        break;

                    case "profile":
                        link.SetProfile(value);
                        break;

                    case "title":
                        link.Title = value;
                        break;

                    case "hreflang":
                        link.HrefLang = value;
                        break;

                }

            }

            if (link.Templated) {

                link.Template = href;

            } else {

                link.SetHref(href);

            }

            return link;

        }

        private static IEnumerable<T> ParseObjectOrArrayOfObjects<T>(JObject outer, Func<JObject, String, T> factory) {

            foreach (var inner in outer.Properties()) {

                var rel = inner.Name;

                if (inner.Value.Type == JTokenType.Array) {

                    foreach (var child in inner.Value.Children<JObject>()) {

                        yield return factory(child, rel);

                    }

                } else {

                    yield return factory((JObject)inner.Value, rel);

                }

            }

        }

        public static void Dump(dynamic data) {

            DumpProperties(data);

            if (data.HasLinks) {

                DumpLinks(data.Links);

            }

            if (data.HasEmbedded) {

                DumpEmbedded(data.Embedded);

            }

        }

        private static void DumpLinks(dynamic links) {

            System.Console.WriteLine("Links");

            foreach (var link in links) {

                System.Console.WriteLine("    title: {0}, Rel: {1}, Href: {2}",
                   link.Title, link.Rel, link.Href);

            }

        }

        private static void DumpEmbedded(dynamic embedded) {

            System.Console.WriteLine("Embedded");

            foreach (var data in embedded) {

                DumpProperties(data);

                if (data.HasLinks) {

                    DumpLinks(data.Links);

                }

            }

        }

        private static void DumpProperties(dynamic data) {

            System.Console.WriteLine("Properties");

            foreach (var property in data.Properties) {

                var type = data.GetPropertyType(property);

                if (type == "Array") {

                    dynamic array = data.GetProperty(property);
                    System.Console.WriteLine("  {0}", property);

                    DumpProperties(array);

                } else {

                    System.Console.WriteLine("  {0}: {1}", property, data.GetProperty(property));

                }

            }

        }

    }

}
