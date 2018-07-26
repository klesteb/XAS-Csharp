using System;

using XAS.Core.DataStructures;

namespace XAS.Rest.Client.Serializers.Lib {

    public class JsonResponse: DynamicObjects {

        public String GetPropertyType(String name) {

            string type = null;

            if (properties.ContainsKey(name)) {

                try {

                    var state = (StateValue)properties[name];
                    type = state.Type;

                } catch { }

            }

            return type;

        }

        public override Object GetProperty(String name) {

            object value = null;

            if (properties.ContainsKey(name)) {

                try {

                    // try to convert to appropiate data type

                    var state = (StateValue)properties[name];

                    switch (state.Type) {
                        case "Integer":
                            value = Convert.ToInt32(state.Value);
                            break;

                        case "Float":
                            value = Convert.ToSingle(state.Value);
                            break;

                        case "Boolean":
                            value = Convert.ToBoolean(state.Value);
                            break;

                        case "Date":
                            value = Convert.ToDateTime(state.Value);
                            break;

                        case "Uri":
                            value = new Uri((string)state.Value);
                            break;

                        default:
                            value = state.Value;
                            break;

                    }

                } catch {

                    value = properties[name];

                }

            }

            return value;

        }

    }

}